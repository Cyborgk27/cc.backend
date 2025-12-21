using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistences.Contexts.Configurations
{
    public class ProjectCatalogConfiguration : IEntityTypeConfiguration<ProjectCatalog>
    {
        public void Configure(EntityTypeBuilder<ProjectCatalog> builder)
        {
            builder.ToTable("ProjectCatalogs");

            builder.HasKey(pc => pc.Id);

            // Configuración de relación con Project
            builder.HasOne(pc => pc.Project)
                   .WithMany(p => p.ProjectCatalogs)
                   .HasForeignKey(pc => pc.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configuración de relación con Catalog
            builder.HasOne(pc => pc.Catalog)
                   .WithMany(c => c.ProjectCatalogs)
                   .HasForeignKey(pc => pc.CatalogId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Auditoría
            builder.Property(pc => pc.AuditCreateUser).IsRequired();
            builder.Property(pc => pc.AuditCreateDate).IsRequired();
            builder.Property(pc => pc.IsDeleted).IsRequired();
        }
    }
}
