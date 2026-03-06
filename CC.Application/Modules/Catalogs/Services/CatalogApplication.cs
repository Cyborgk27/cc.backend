using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Catalog;
using CC.Application.Modules.Catalogs.Interfaces;
using CC.Domain.Entities;
using CC.Domain.Exceptions;
using CC.Domain.Repositories;
using CC.Utilities.Static;

namespace CC.Application.Modules.Catalogs.Services
{
    public class CatalogApplication : ICatalogApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ServiceData _serviceData;

        public CatalogApplication(IUnitOfWork unitOfWork, ServiceData serviceData)
        {
            _unitOfWork = unitOfWork;
            _serviceData = serviceData;
        }

        public async Task<BaseResponse<bool>> SaveCatalogAsync(CatalogDto dto)
        {
            Catalog catalog;

            // 1. PROCESAR CATÁLOGO PRINCIPAL (O HIJO INDIVIDUAL)
            if (dto.Id.HasValue && dto.Id > 0)
            {
                catalog = await _unitOfWork.Catalogs.GetByIdAsync(dto.Id.Value);
                if (catalog == null) throw new EntityNotFoundException("Catalog", dto.Id.Value);

                catalog.UpdateInfo(dto.ShowName, dto.Abbreviation, dto.Value, dto.ParentId, dto.Description);
            }
            else
            {
                catalog = new Catalog(dto.Name, dto.ShowName, dto.Abbreviation, dto.Value, dto.Description, dto.ParentId);
                await _unitOfWork.Catalogs.AddAsync(catalog);
            }

            // 2. PROCESAR HIJOS (Si vienen en el DTO)
            if (dto.Children != null && dto.Children.Any())
            {
                foreach (var childDto in dto.Children)
                {
                    if (childDto.IsDeleted && childDto.Id > 0)
                    {
                        var childToDelete = await _unitOfWork.Catalogs.GetByIdAsync(childDto.Id.Value);
                        if (childToDelete != null) await _unitOfWork.Catalogs.DeleteAsync(childToDelete);
                    }
                    else if (childDto.Id == null || childDto.Id == 0)
                    {
                        // Crear nuevo hijo usando el método de dominio del padre
                        var newChild = new Catalog(childDto.Name, childDto.ShowName, childDto.Abbreviation, childDto.Value, childDto.Description, null);
                        catalog.AddChild(newChild);
                        await _unitOfWork.Catalogs.AddAsync(newChild);
                    }
                    else if (childDto.Id > 0)
                    {
                        // Lógica opcional para actualizar los datos del hijo existente
                        var existingChild = catalog.Children.FirstOrDefault(c => c.Id == childDto.Id);
                        existingChild?.UpdateInfo(childDto.ShowName, childDto.Abbreviation, childDto.Value, childDto.ParentId, childDto.Description);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_SAVE);
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

        public async Task<BaseResponse<CatalogDto>> GetCatalogByIdAsync(int id)
        {
            var catalog = await _unitOfWork.Catalogs.GetCatalogDetailsAsync(id);
            if (catalog == null) throw new EntityNotFoundException("Catalog", id);
            var dto = MapToDto(catalog, true);
            return _serviceData.CreateResponse(dto, ReplyMessage.MESSAGE_QUERY);
        }

        public async Task<BaseResponse<bool>> DeleteCatalogAsync(int id)
        {
            var catalog = await _unitOfWork.Catalogs.GetByIdAsync(id);
            if (catalog == null) throw new EntityNotFoundException("Catalog", id);
            await _unitOfWork.Catalogs.DeleteAsync(catalog);
            await _unitOfWork.SaveChangesAsync();
            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_DELETE);
        }

        // Helper para mapeo recursivo
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
