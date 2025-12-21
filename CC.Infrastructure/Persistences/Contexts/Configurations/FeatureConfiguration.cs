using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistences.Contexts.Configurations
{
    internal class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            // Nombre de la tabla (opcional, por defecto es el nombre del DbSet)
            builder.ToTable("Features");

            // Clave primaria
            builder.HasKey(f => f.Id);

            // Regla de Negocio: El 'Name' debe ser único a nivel de base de datos
            builder.HasIndex(f => f.Name)
                   .IsUnique();

            // Configuraciones de propiedades
            builder.Property(f => f.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(f => f.ShowName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(f => f.Path)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(f => f.Icon)
                   .HasMaxLength(50);

            // Relación: Un Feature tiene muchos AvailablePermissions
            builder.HasMany(f => f.AvailablePermissions)
                   .WithOne() // Relación unidireccional o bidireccional según tu clase Permission
                   .HasForeignKey(p => p.FeatureId)
                   .OnDelete(DeleteBehavior.Cascade); // Si borras el Feature, se borran sus permisos

            // Ignorar el filtrado automático de auditoría si fuera necesario, 
            // pero aquí mapeamos los campos de BaseEntity
            builder.Property(f => f.IsDeleted).IsRequired();
        }
    }
}