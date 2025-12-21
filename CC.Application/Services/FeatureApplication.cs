using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Features;
using CC.Application.Interfaces;
using CC.Domain.Entities;
using CC.Domain.Exceptions;
using CC.Domain.Repositories;
using CC.Utilities.Static;

namespace CC.Application.Services
{
    public class FeatureApplication : IFeatureApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ServiceData _serviceData;

        public FeatureApplication(IUnitOfWork unitOfWork, ServiceData serviceData)
        {
            _unitOfWork = unitOfWork;
            _serviceData = serviceData;
        }

        public async Task<BaseResponse<IEnumerable<FeatureDto>>> GetPagedFeaturesAsync(int page, int size, string? name = null)
        {
            var pagedResult = await _unitOfWork.Features.GetPagedAsync(
                page,
                size,
                filter: x => (string.IsNullOrEmpty(name) || x.Name.Contains(name)) && !x.IsDeleted,
                orderBy: x => x.OrderBy(f => f.ShowName)
            );

            // Mapeo manual (o puedes usar AutoMapper)
            var dtos = pagedResult.Items.Select(x => new FeatureDto(
                x.Id, x.Name, x.ShowName, x.Path, x.Icon
            ));

            return _serviceData.CreateResponse(dtos, ReplyMessage.MESSAGE_QUERY, 200, pagedResult.TotalCount);
        }

        public async Task<BaseResponse<FeatureDto>> GetFeatureByIdAsync(int id)
        {
            var feature = await _unitOfWork.Features.GetByIdAsync(id);

            if (feature == null || feature.IsDeleted)
                throw new EntityNotFoundException("Feature", id);

            var dto = new FeatureDto(feature.Id, feature.Name, feature.ShowName, feature.Path, feature.Icon);
            return _serviceData.CreateResponse(dto);
        }

        public async Task<BaseResponse<bool>> CreateFeatureAsync(CreateFeatureRequest request)
        {
            // La entidad Feature valida sus propias reglas en el constructor (DomainException)
            var newFeature = new Feature(request.Name, request.ShowName, request.Path, request.Icon);

            await _unitOfWork.Features.AddAsync(newFeature);
            await _unitOfWork.SaveChangesAsync();

            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_SAVE, 201);
        }

        public async Task<BaseResponse<bool>> UpdateFeatureAsync(UpdateFeatureRequest request)
        {
            var feature = await _unitOfWork.Features.GetByIdAsync(request.Id);

            if (feature == null) throw new EntityNotFoundException("Feature", request.Id);

            // Usamos el método rico de la entidad que creamos antes
            feature.UpdateDetails(request.ShowName, request.Path, request.Icon);

            await _unitOfWork.Features.UpdateAsync(feature);
            await _unitOfWork.SaveChangesAsync();

            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_UPDATE);
        }

        public async Task<BaseResponse<bool>> DeleteFeatureAsync(int id)
        {
            var feature = await _unitOfWork.Features.GetByIdAsync(id);

            if (feature == null) throw new EntityNotFoundException("Feature", id);

            await _unitOfWork.Features.DeleteAsync(feature);
            await _unitOfWork.SaveChangesAsync();

            return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_DELETE);
        }
    }
}