using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.External;
using CC.Application.Interfaces;
using CC.Domain.Repositories;

namespace CC.Application.Services
{
    public class ExternalCatalogApplication : IExternalCatalogApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ServiceData _serviceData;

        public ExternalCatalogApplication(IUnitOfWork unitOfWork, ServiceData serviceData)
        {
            _unitOfWork = unitOfWork;
            _serviceData = serviceData;
        }

        public async Task<BaseResponse<IEnumerable<ExternalCatalogResponse>>> GetCatalogsByProjectIdAsync(Guid projectId)
        {
            // 1. Llamamos al método especializado del repositorio
            var projectCatalogs = await _unitOfWork.ProjectCatalogs.GetCatalogsByProjectIdAsync(projectId);

            // 2. Mapeamos a la respuesta DTO
            var data = projectCatalogs.Select(pc => new ExternalCatalogResponse(
                pc.Catalog.Name,
                pc.Catalog.ShowName,
                pc.Catalog.Value,
                pc.Catalog.Abbreviation,
                pc.Catalog.Description,
                Childrens: pc.Catalog.Children?
                    .Where(child => !child.IsDeleted) // Filtramos hijos no eliminados
                    .Select(child => new ExternalCatalogResponse(
                        child.Name,
                        child.ShowName,
                        child.Value,
                        child.Abbreviation,
                        child.Description,
                        null // En este nivel de lista general, los hijos de los hijos suelen ir nulos
                    )).ToList()
            ));

            // 3. Retornamos usando ServiceData para estandarizar
            return _serviceData.CreateResponse(data, "Catálogos recuperados exitosamente vía API Key.");
        }
    }
}