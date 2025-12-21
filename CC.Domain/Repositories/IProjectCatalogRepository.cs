using CC.Domain.Entities;

namespace CC.Domain.Repositories
{
    public interface IProjectCatalogRepository : IGenericRepository<ProjectCatalog, int>
    {
        Task<IEnumerable<ProjectCatalog>> GetCatalogsByProjectIdAsync(Guid projectId);
    }
}
