using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectApiKeyConfiguration : IEntityTypeConfiguration<ProjectApiKey>
{
    public void Configure(EntityTypeBuilder<ProjectApiKey> builder)
    {
        builder.ToTable("ProjectApiKeys");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Key).IsRequired().HasMaxLength(100);

        // Índice único para la llave para búsquedas rápidas en el Middleware
        builder.HasIndex(x => x.Key).IsUnique();

        builder.HasOne(x => x.Project)
               .WithMany(p => p.ApiKeys)
               .HasForeignKey(x => x.ProjectId);
    }
}