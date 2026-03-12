using CC.Domain.Entities.Catalogs;

namespace CC.Domain.Repositories
{
    public interface ICatalogRepository : IGenericRepository<Catalog, int>
    {
        Task<Catalog?> GetCatalogDetailsAsync(int id);
    }
}
