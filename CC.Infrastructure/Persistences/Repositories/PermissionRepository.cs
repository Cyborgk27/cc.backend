using CC.Domain.Entities;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class PermissionRepository : GenericRepository<Permission, int>, IPermissionRepository
    {
        public PermissionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
