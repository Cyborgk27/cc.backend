using CC.Application.Modules.Identity.Interfaces;
using CC.Domain.Entities.Identity;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class PermissionRepository : GenericRepository<Permission, int>, IPermissionRepository
    {
        public PermissionRepository(AppDbContext context, IUserContext userContext) : base(context, userContext)
        {
        }
    }
}
