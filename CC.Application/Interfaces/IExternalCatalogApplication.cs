using CC.Application.Common.Bases;
using CC.Application.DTOs.External;

namespace CC.Application.Interfaces
{
    public interface IExternalCatalogApplication
    {
        Task<BaseResponse<IEnumerable<ExternalCatalogResponse>>> GetCatalogsByProjectIdAsync(Guid projectId);
    }
}
