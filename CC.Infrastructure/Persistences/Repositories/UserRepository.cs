using CC.Domain.Entities;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Persistences.Repositories
{
    internal class UserRepository : GenericRepository<User, Guid>, IUserRepository
    {
        public UserRepository(AppDbContext context, Application.Interfaces.IUserContext userContext) : base(context, userContext)
        {
        }

        public async Task<bool> HasPermission(Guid userId, string featureName, string actionName)
        {
            // 1. Normalizamos
            var fName = featureName.ToUpper().Trim();
            var aName = actionName.ToUpper().Trim();

            // 2. Creamos el nombre técnico tal como está en la DB: "CATALOGS_READ"
            var fullPermissionName = $"{fName}_{aName}";

            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId && !u.IsDeleted)
                .SelectMany(u => u.Role.RolePermissions)
                .AnyAsync(rp =>
                    !rp.IsDeleted &&
                    rp.Permission.Name == fullPermissionName // <--- Comparamos contra el Name compuesto
                );
        }

        public async Task<User?> GetUserWithRoleAsync(Guid id)
        {
            return await _context.Set<User>()
                .Include(u => u.Role) // Trae la información del Rol (Admin, Editor, etc.)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }
    }
}
