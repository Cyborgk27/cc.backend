using CC.Application.Modules.Identity.Interfaces;
using CC.Domain.Entities.System;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class SystemAuditRepository : GenericRepository<SystemAudit, Guid>, ISystemAuditRepository
    {
        public SystemAuditRepository(AppDbContext context, IUserContext userContext) : base(context, userContext)
        {
        }
    }
}
