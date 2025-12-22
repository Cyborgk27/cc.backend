using CC.Domain.Entities;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class RolePermissionRepository : GenericRepository<RolePermission, int>, IRolePermissionRepository
    {
        public RolePermissionRepository(AppDbContext context, Application.Interfaces.IUserContext userContext) : base(context, userContext)
        {
        }
    }
}
