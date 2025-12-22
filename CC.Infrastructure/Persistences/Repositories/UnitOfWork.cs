using CC.Application.Interfaces;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IUserContext _userContext;
        public IFeatureRepository Features { get; private set; }

        public IRoleRepository Roles { get; private set; }

        public IRolePermissionRepository RolePermissions { get; private set; }

        public IPermissionRepository Permissions { get; private set; }

        public IUserRepository Users { get; private set; }

        public IProjectRepository Projects { get; private set; }

        public IProjectCatalogRepository ProjectCatalogs { get; private set; }

        public IProjectApiKeyRepository ProjectApiKeys { get; private set; }

        public ICatalogRepository Catalogs { get; private set; }

        public UnitOfWork(AppDbContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
            Features = new FeatureRepository(_context, _userContext);
            Permissions = new PermissionRepository(_context, _userContext);
            Roles = new RoleRepository(_context, _userContext);
            RolePermissions = new RolePermissionRepository(_context, _userContext);
            Users = new UserRepository(_context, _userContext);
            Projects = new ProjectRepository(_context, _userContext);
            ProjectCatalogs = new ProjectCatalogRepository(_context, _userContext);
            ProjectApiKeys = new ProjectApiKeyRepository(_context, _userContext);
            Catalogs = new CatalogRepository(_context, _userContext);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
