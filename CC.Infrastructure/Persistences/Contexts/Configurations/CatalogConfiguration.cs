using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistence.Configurations
{
    public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
            builder.ToTable("Catalogs");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ShowName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Abbreviation)
                .HasMaxLength(20);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            // ================================================================
            // RELACIÓN JERÁRQUICA (Auto-referencia)
            // ================================================================

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Evita borrar hijos accidentalmente

            // ================================================================
            // ÍNDICES PARA RENDIMIENTO
            // ================================================================

            builder.HasIndex(x => x.Name).IsUnique(); // El nombre técnico debe ser único
            builder.HasIndex(x => x.ParentId);
        }
    }
}