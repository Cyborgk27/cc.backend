using CC.Application.Modules.Identity.Interfaces;
using CC.Domain.Entities.Project;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class ProjectRepository : GenericRepository<Project, Guid>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context, IUserContext userContext) : base(context, userContext)
        {
        }

        public async Task<Project?> GetProjectDetailsAsync(Guid id)
        {
            return await _context.Set<Project>()
                .Include(p => p.ApiKeys) // Aquí sí usamos el Include sin ensuciar Application
                .Include(p => p.ProjectCatalogs)
                    .ThenInclude(pc => pc.Catalog)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }
    }
}