using CC.Domain.Entities;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Persistences.Repositories
{
    internal class ProjectApiKeyRepository : GenericRepository<ProjectApiKey, int>, IProjectApiKeyRepository
    {
        public ProjectApiKeyRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ProjectApiKey?> GetActiveKeyWithProjectAsync(string key)
        {
            return await _context.Set<ProjectApiKey>()
                .Include(x => x.Project)
                .Where(x => x.Key == key &&
                            !x.IsDeleted &&
                            x.IsEnabled)
                .FirstOrDefaultAsync();
        }
    }
}