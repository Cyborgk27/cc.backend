using CC.Domain.Entities;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class RoleRepository : GenericRepository<Role, Guid>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }
    }
}
