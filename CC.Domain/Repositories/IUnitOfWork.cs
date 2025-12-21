namespace CC.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IFeatureRepository Features { get; }
        IRoleRepository Roles { get; }
        IRolePermissionRepository RolePermissions { get; }
        IPermissionRepository Permissions { get; }
        IUserRepository Users { get; }
        IProjectRepository Projects { get; }
        IProjectCatalogRepository ProjectCatalogs { get; }
        IProjectApiKeyRepository ProjectApiKeys { get; }
        ICatalogRepository Catalogs { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
