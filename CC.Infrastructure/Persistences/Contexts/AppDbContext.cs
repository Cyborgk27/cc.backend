using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Persistences.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Esto aplica todas las configuraciones que crearemos en la carpeta 'Configurations'
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    // El interceptor de auditoría que te mencioné antes (Savechanges automático)
    //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    //{
    //    foreach (var entry in ChangeTracker.Entries<CC.Domain.Common.BaseEntity<Guid>>())
    //    {
    //        // Aquí podrías usar un servicio para obtener el ID del usuario actual
    //        var userId = Guid.Empty;

    //        if (entry.State == EntityState.Added)
    //            entry.Entity.MarkAsCreated(userId);
    //        else if (entry.State == EntityState.Modified)
    //            entry.Entity.MarkAsUpdated(userId);
    //        else if (entry.State == EntityState.Deleted)
    //        {
    //            entry.State = EntityState.Modified;
    //            entry.Entity.MarkAsDeleted(userId);
    //        }
    //    }
    //    return base.SaveChangesAsync(cancellationToken);
    //}
}