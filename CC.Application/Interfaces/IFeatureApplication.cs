using CC.Application.Common.Bases;
using CC.Application.DTOs.Features;

namespace CC.Application.Interfaces
{
    public interface IFeatureApplication
    {
        // Consulta paginada con DTOs
        Task<BaseResponse<IEnumerable<FeatureDto>>> GetPagedFeaturesAsync(int page, int size, string? name = null);

        Task<BaseResponse<FeatureDto>> GetFeatureByIdAsync(int id);

        Task<BaseResponse<bool>> CreateFeatureAsync(CreateFeatureRequest request);

        Task<BaseResponse<bool>> UpdateFeatureAsync(UpdateFeatureRequest request);

        Task<BaseResponse<bool>> DeleteFeatureAsync(int id);
    }
}
