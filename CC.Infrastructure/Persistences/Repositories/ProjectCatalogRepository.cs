using CC.Domain.Entities;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class ProjectCatalogRepository : GenericRepository<ProjectCatalog, int>, IProjectCatalogRepository
    {
        public ProjectCatalogRepository(AppDbContext context, Application.Interfaces.IUserContext userContext) : base(context, userContext)
        {
        }

        public async Task<IEnumerable<ProjectCatalog>> GetCatalogsByProjectIdAsync(Guid projectId)
        {
            return await _context.Set<ProjectCatalog>()
                .Include(pc => pc.Catalog)
                .ThenInclude(c => c.Children)
                .Where(pc => pc.ProjectId == projectId && !pc.IsDeleted)
                .ToListAsync();
        }

        public async Task<Catalog?> GetCatalogByCode(Guid projectId, string abbreviation)
        {
            return await _context.Set<ProjectCatalog>()
                .Include(pc => pc.Catalog)
                .ThenInclude(c => c.Children)
                .Where(pc => pc.ProjectId == projectId && !pc.IsDeleted && pc.Catalog.Abbreviation == abbreviation)
                .Select(pc => pc.Catalog)
                .FirstOrDefaultAsync();
        }
    }
}