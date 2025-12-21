using CC.Domain.Entities;

namespace CC.Domain.Repositories
{
    public interface ICatalogRepository : IGenericRepository<Catalog, int>
    {
        Task<Catalog?> GetCatalogDetailsAsync(int id);
    }
}
