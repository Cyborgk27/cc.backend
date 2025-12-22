using CC.Domain.Entities;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class CatalogRepository : GenericRepository<Catalog, int>, ICatalogRepository
    {
        public CatalogRepository(AppDbContext context, Application.Interfaces.IUserContext userContext) : base(context, userContext)
        {
        }

        public async Task<Catalog?> GetCatalogDetailsAsync(int id)
        {
            return await _context.Set<Catalog>()
                .Include(c => c.Children) // Carga las opciones del catálogo
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}