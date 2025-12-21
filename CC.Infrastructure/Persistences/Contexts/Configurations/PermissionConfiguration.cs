using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistences.Contexts.Configurations
{
    internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.Id);

            // Regla de Negocio: El código técnico del permiso (Name) debe ser único
            // para que el sistema de Claims no tenga ambigüedades.
            builder.HasIndex(p => p.Name)
                   .IsUnique();

            builder.Property(p => p.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(p => p.ShowName)
                   .HasMaxLength(100)
                   .IsRequired();

            // Configuración de la relación con Feature
            builder.HasOne(p => p.Feature)
               .WithMany(f => f.AvailablePermissions)
               .HasForeignKey(p => p.FeatureId)
               .OnDelete(DeleteBehavior.Cascade);

            // Auditoría (Heredada de BaseEntity)
            builder.Property(p => p.AuditCreateUser).IsRequired();
            builder.Property(p => p.AuditCreateDate).IsRequired();
            builder.Property(p => p.IsDeleted).IsRequired();
        }
    }
}