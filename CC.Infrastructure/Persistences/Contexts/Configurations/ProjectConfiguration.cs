using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistences.Contexts.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);

            // Regla de Negocio: El nombre técnico debe ser único
            builder.HasIndex(p => p.Name)
                   .IsUnique();

            builder.Property(p => p.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(p => p.ShowName)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasMaxLength(500);

            builder.Property(p => p.IsActive)
                   .HasDefaultValue(true);

            // Relación con ProjectCatalog
            // Usamos el campo privado _projectCatalogs si es necesario, 
            // pero normalmente EF mapea la propiedad pública.
            builder.HasMany(p => p.ProjectCatalogs)
                   .WithOne(pc => pc.Project)
                   .HasForeignKey(pc => pc.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Auditoría
            builder.Property(p => p.AuditCreateUser).IsRequired();
            builder.Property(p => p.AuditCreateDate).IsRequired();
            builder.Property(p => p.IsDeleted).IsRequired();
        }
    }
}