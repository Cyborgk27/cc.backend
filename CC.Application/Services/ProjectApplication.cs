using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Project;
using CC.Application.DTOs.Project.CC.Application.DTOs.Project;
using CC.Application.Interfaces;
using CC.Domain.Entities;
using CC.Domain.Exceptions;
using CC.Domain.Repositories;
using CC.Utilities.Static;
using System.Linq;

namespace CC.Application.Services
{
    public class ProjectApplication : IProjectApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ServiceData _serviceData;

        public ProjectApplication(IUnitOfWork unitOfWork, ServiceData serviceData)
        {
            _unitOfWork = unitOfWork;
            _serviceData = serviceData;
        }

        public async Task<BaseResponse<IEnumerable<ProjectDto>>> GetPagedProjectsAsync(int page, int size, string? name = null)
        {
            var pagedResult = await _unitOfWork.Projects.GetPagedAsync(
                page,
                size,
                filter: x => (string.IsNullOrEmpty(name) || x.Name.Contains(name)) && !x.IsDeleted,
                orderBy: x => x.OrderBy(f => f.ShowName)
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
            // Usamos el repositorio específico que ya incluye .Include(p => p.ApiKeys).Include(p => p.ProjectCatalogs)
            var project = await _unitOfWork.Projects.GetProjectDetailsAsync(id);

            if (project == null || project.IsDeleted)
                throw new EntityNotFoundException("Project", id);

            var dto = new ProjectDto(
                project.Id,
                project.Name,
                project.ShowName,
                project.Description,
                project.IsDeleted,
                project.ApiKeys.Where(k => !k.IsDeleted).Select(k => new ProjectApiKeyDto(
                    k.Id,
                    k.Title,
                    k.Description,
                    k.Key,
                    k.ExpirationDate,
                    k.IsIndefinite,
                    k.AllowedIp,
                    k.AllowedDomain,
                    false // IsDeleted false por defecto al consultar
                )).OrderByDescending(k => k.Id),
                project.ProjectCatalogs.Select(pc => pc.CatalogId) // Lista de IDs de catálogos
            );

            return _serviceData.CreateResponse(dto, ReplyMessage.MESSAGE_QUERY);
        }

        public async Task<BaseResponse<bool>> SaveProjectAsync(ProjectDto dto)
        {
            Project project;

            // 1. DETERMINAR SI ES PROYECTO NUEVO O EXISTENTE
            if (dto.Id.HasValue && dto.Id != Guid.Empty)
            {
                project = await _unitOfWork.Projects.GetProjectDetailsAsync(dto.Id.Value);
                if (project == null) throw new EntityNotFoundException("Project", dto.Id.Value);

                project.UpdateInfo(dto.Name, dto.ShowName, dto.Description);
            }
            else
            {
                project = new Project(dto.Name, dto.ShowName, dto.Description);
                await _unitOfWork.Projects.AddAsync(project);
            }

            // 2. PROCESAR API KEYS (Sincronización)
            if (dto.ApiKeys != null)
            {
                foreach (var keyDto in dto.ApiKeys)
                {
                    // CASO A: MARCAR PARA ELIMINAR
                    if (keyDto.IsDeleted && keyDto.Id > 0)
                    {
                        var keyToDelete = project.ApiKeys.FirstOrDefault(k => k.Id == keyDto.Id);
                        if (keyToDelete != null)
                        {
                            keyToDelete.Revoke(); // Método de dominio para desactivar
                            await _unitOfWork.ProjectApiKeys.DeleteAsync(keyToDelete); // Borrado lógico en repo
                        }
                    }
                    // CASO B: ACTUALIZAR EXISTENTE
                    else if (keyDto.Id > 0)
                    {
                        var keyToUpdate = project.ApiKeys.FirstOrDefault(k => k.Id == keyDto.Id);
                        if (keyToUpdate != null)
                        {
                            keyToUpdate.UpdateSecurity(keyDto.AllowedIp, keyDto.AllowedDomain);
                            // Aquí podrías añadir más métodos de actualización si la entidad lo permite
                        }
                    }
                    // CASO C: CREAR NUEVA (ID es null o 0)
                    else if (!keyDto.IsDeleted)
                    {
                        project.GenerateApiKey(
                            keyDto.Title,
                            keyDto.Description,
                            keyDto.ExpirationDate,
                            keyDto.IsIndefinite,
                            keyDto.AllowedIp,
                            keyDto.AllowedDomain
                        );
                    }
                }
            }

            // 3. PROCESAR CATÁLOGOS (Asignación)
            if (dto.CatalogIds != null)
            {
                foreach (var catalogId in dto.CatalogIds)
                {
                    if (!project.ProjectCatalogs.Any(pc => pc.CatalogId == catalogId))
                        project.AssignCatalog(catalogId);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_SAVE);
        }
    }
}