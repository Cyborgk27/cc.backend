using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistences.Contexts.Configurations
{
    internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");

            builder.HasKey(rp => rp.Id);

            // REGLA DE ORO: Un rol no puede tener el mismo permiso asignado más de una vez.
            // Creamos un índice único compuesto entre RoleId y PermissionId.
            builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                   .IsUnique();

            // Relación con Role
            builder.HasOne(rp => rp.Role)
                   .WithMany(r => r.RolePermissions)
                   .HasForeignKey(rp => rp.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relación con Permission
            builder.HasOne(rp => rp.Permission)
                   .WithMany() // Permission no necesariamente necesita una lista de RolePermissions
                   .HasForeignKey(rp => rp.PermissionId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Restrict: No permitimos borrar un permiso si hay roles usándolo.
        }
    }
}