using CC.Application.Common.Bases;
using CC.Application.DTOs.Catalog;

namespace CC.Application.Interfaces
{
    public interface ICatalogApplication
    {
        Task<BaseResponse<bool>> SaveCatalogAsync(CatalogDto dto);
        Task<BaseResponse<IEnumerable<CatalogDto>>> GetPagedCatalogsAsync(int page, int size, string? name = null);
        Task<BaseResponse<CatalogDto>> GetCatalogByIdAsync(int id);
    }
}
