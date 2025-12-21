using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IFeatureRepository Features { get; private set; }

        public IRoleRepository Roles { get; private set; }

        public IRolePermissionRepository RolePermissions { get; private set; }

        public IPermissionRepository Permissions { get; private set; }

        public IUserRepository Users { get; private set; }

        public IProjectRepository Projects { get; private set; }

        public IProjectCatalogRepository ProjectCatalogs { get; private set; }

        public IProjectApiKeyRepository ProjectApiKeys { get; private set; }

        public ICatalogRepository Catalogs { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Features = new FeatureRepository(_context);
            Permissions = new PermissionRepository(_context);
            Roles = new RoleRepository(_context);
            RolePermissions = new RolePermissionRepository(_context);
            Users = new UserRepository(_context);
            Projects = new ProjectRepository(_context);
            ProjectCatalogs = new ProjectCatalogRepository(_context);
            ProjectApiKeys = new ProjectApiKeyRepository(_context);
            Catalogs = new CatalogRepository(_context);
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
