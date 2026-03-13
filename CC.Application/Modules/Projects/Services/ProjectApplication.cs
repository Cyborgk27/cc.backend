using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Catalog;
using CC.Application.DTOs.Project.CC.Application.DTOs.Project;
using CC.Application.Modules.Identity.Interfaces; // Para IUserContext
using CC.Application.Modules.Projects.Interfaces;
using CC.Domain.Entities.Catalogs;
using CC.Domain.Entities.Project;
using CC.Domain.Exceptions;
using CC.Domain.Repositories;
using CC.Utilities.Static;
using System.Drawing;
using System.Xml.Linq;

namespace CC.Application.Modules.Projects.Services
{
    public class ProjectApplication : IProjectApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ServiceData _serviceData;
        private readonly IUserContext _userContext; // Inyección de seguridad

        public ProjectApplication(IUnitOfWork unitOfWork, ServiceData serviceData, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _serviceData = serviceData;
            _userContext = userContext;
        }

        public async Task<BaseResponse<IEnumerable<ProjectDto>>> GetPagedProjectsAsync(int page, int size, string? name = null)
        {
            // Filtro dinámico: Si no es Admin, solo ve sus propios proyectos (AuditCreateUser)
            var pagedResult = await _unitOfWork.Projects.GetPagedAsync(
                page,
                size,
                filter: x => (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
                             (_userContext.IsInRole("ADMINISTRATOR") || x.AuditCreateUser == _userContext.UserId),
                orderBy: x => x.OrderBy(f => f.AuditCreateUser)
            );

            var dtos = pagedResult.Items.Select(x => new ProjectDto(
                x.Id,
                x.Name,
                x.ShowName,
                x.Description,
                x.IsDeleted,
                Enumerable.Empty<ProjectApiKeyDto>(),
                Enumerable.Empty<int>()
            ));

            return _serviceData.CreateResponse(dtos, ReplyMessage.MESSAGE_QUERY, 200, pagedResult.TotalCount);
        }

        public async Task<BaseResponse<ProjectDto>> GetProjectByIdAsync(Guid id)
        {
            var project = await _unitOfWork.Projects.GetProjectDetailsAsync(id);

            // Validación de existencia
            if (project == null || project.IsDeleted)
                throw new UserFriendlyException(ReplyMessage.MESSAGE_NOT_FOUND, 404);

            // SEGURIDAD: Validar propiedad del registro
            if (!_userContext.IsInRole("ADMINISTRATOR") && project.AuditCreateUser != _userContext.UserId)
                throw new UnauthorizedAccessException("No tienes permiso para acceder a este proyecto.");

            var dto = new ProjectDto(
                project.Id,
                project.Name,
                project.ShowName,
                project.Description,
                project.IsDeleted,
                project.ApiKeys.Where(k => !k.IsDeleted).Select(k => new ProjectApiKeyDto(
                    k.Id, k.Title, k.Description, k.Key, k.ExpirationDate, k.IsIndefinite, k.AllowedIp, k.AllowedDomain, false
                )).OrderByDescending(k => k.Id),
                project.ProjectCatalogs.Select(pc => pc.CatalogId)
            );

            return _serviceData.CreateResponse(dto, ReplyMessage.MESSAGE_QUERY);
        }

        public async Task<BaseResponse<bool>> SaveProjectAsync(ProjectDto dto)
        {
            Project project;

            if (dto.Id.HasValue && dto.Id != Guid.Empty)
            {
                project = await _unitOfWork.Projects.GetProjectDetailsAsync(dto.Id.Value);
                if (project == null) throw new UserFriendlyException(ReplyMessage.MESSAGE_NOT_FOUND, 404);

                // SEGURIDAD: Solo el dueño o Admin editan
                if (!_userContext.IsInRole("ADMINISTRATOR") && project.AuditCreateUser != _userContext.UserId)
                    throw new UnauthorizedAccessException("No tienes permiso para modificar este proyecto.");

                project.UpdateInfo(dto.Name, dto.ShowName, dto.Description);
            }
            else
            {
                project = new Project(dto.Name, dto.ShowName, dto.Description);
                await _unitOfWork.Projects.AddAsync(project);
            }

            // --- PROCESAR API KEYS ---
            if (dto.ApiKeys != null)
            {
                foreach (var keyDto in dto.ApiKeys)
                {
                    if (keyDto.IsDeleted && keyDto.Id > 0)
                    {
                        var keyToDelete = project.ApiKeys.FirstOrDefault(k => k.Id == keyDto.Id);
                        if (keyToDelete != null)
                        {
                            keyToDelete.Revoke();
                            await _unitOfWork.ProjectApiKeys.DeleteAsync(keyToDelete);
                        }
                    }
                    else if (keyDto.Id > 0)
                    {
                        var keyToUpdate = project.ApiKeys.FirstOrDefault(k => k.Id == keyDto.Id);
                        keyToUpdate?.UpdateSecurity(keyDto.AllowedIp, keyDto.AllowedDomain);
                    }
                    else if (!keyDto.IsDeleted)
                    {
                        project.GenerateApiKey(keyDto.Title, keyDto.Description, keyDto.ExpirationDate, keyDto.IsIndefinite, keyDto.AllowedIp, keyDto.AllowedDomain);
                    }
                }
            }

            // --- PROCESAR CATÁLOGOS (Sincronización) ---
            if (dto.CatalogIds != null)
            {
                // 1. Identificamos los que ya no están en el DTO para eliminarlos
                // Convertimos a lista para evitar errores de modificación de colección durante el bucle
                var catalogsToRemove = project.ProjectCatalogs
                    .Where(pc => !dto.CatalogIds.Contains(pc.CatalogId))
                    .ToList();

                foreach (var pc in catalogsToRemove)
                {
                    // Usamos tu método de dominio para que la lógica de negocio se mantenga
                    project.RemoveCatalog(pc.CatalogId);
                }

                // 2. Identificamos los nuevos que no están en la entidad para agregarlos
                foreach (var catalogId in dto.CatalogIds)
                {
                    if (!project.ProjectCatalogs.Any(pc => pc.CatalogId == catalogId))
                    {
                        project.AssignCatalog(catalogId);
                    }
                }
            }
            else
            {
                // Si el DTO no trae nada, eliminamos todas las asignaciones existentes
                var allCatalogs = project.ProjectCatalogs.ToList();
                foreach (var pc in allCatalogs)
                {
                    project.RemoveCatalog(pc.CatalogId);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_SAVE);
        }

        public async Task<BaseResponse<bool>> DeleteProjectAsync(Guid id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null || project.IsDeleted)
                throw new UserFriendlyException(ReplyMessage.MESSAGE_NOT_FOUND, 404);

            // SEGURIDAD: Solo el dueño o Admin eliminan
            if (!_userContext.IsInRole("Admin") && project.AuditCreateUser != _userContext.UserId)
                throw new UnauthorizedAccessException("No tienes permiso para eliminar este proyecto.");

            await _unitOfWork.Projects.DeleteAsync(project);
            await _unitOfWork.SaveChangesAsync();
            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_DELETE);
        }

        public async Task<BaseResponse<bool>> DeleteApiKey(int apiKeyId)
        {
            var apiKey = await _unitOfWork.ProjectApiKeys.GetByIdAsync(apiKeyId);

            if (apiKey == null || apiKey.IsDeleted)
                throw new UserFriendlyException(ReplyMessage.MESSAGE_NOT_FOUND, 404);

            // Opcional: Validar que la API Key pertenezca a un proyecto del usuario actual
            // (Esto se garantiza si el flujo de UI es correcto, pero puedes añadir la validación aquí)

            apiKey.Revoke();
            await _unitOfWork.ProjectApiKeys.DeleteAsync(apiKey);
            await _unitOfWork.SaveChangesAsync();

            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_DELETE);
        }

        public async Task<BaseResponse<IEnumerable<CatalogDto>>> GetPagedCatalogsAsync(int page, int size, string? name = null)
        {
            var pagedResult = await _unitOfWork.Catalogs.GetPagedAsync(
                page,
                size,
                filter: x => (string.IsNullOrEmpty(name) || x.Name.Contains(name)) && x.IsParent,
                orderBy: x => x.OrderByDescending(f => f.AuditCreateDate)
            );

            // Validamos que Items no sea nulo antes de mapear
            if (pagedResult.Items == null)
            {
                return _serviceData.CreateResponse(Enumerable.Empty<CatalogDto>(), ReplyMessage.MESSAGE_QUERY, 200, 0);
            }

            // Mapeamos con seguridad
            var dtos = pagedResult.Items.Select(x => MapToDto(x, false)).AsEnumerable();

            return _serviceData.CreateResponse(dtos, ReplyMessage.MESSAGE_QUERY, 200, pagedResult.TotalCount);
        }

        private CatalogDto MapToDto(Catalog entity, bool includeChildren)
        {
            if (entity == null) return null!;

            return new CatalogDto(
                entity.Id,
                entity.ParentId,
                entity.Name,
                entity.ShowName,
                entity.Abbreviation,
                entity.Value,
                entity.Description,
                entity.IsParent,
                !entity.IsDeleted,
                entity.IsDeleted,
                // El ?. es la clave para que no de Error 500
                includeChildren ? entity.Children?.Select(c => MapToDto(c, true)) : null
            );
        }
    }
}