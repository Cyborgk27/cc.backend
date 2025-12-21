using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistences.Contexts.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            // Identidad y Nombres (Nuevos campos)
            builder.Property(u => u.FirstName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.LastName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.UserName) // Tu 'name' técnico
                   .HasMaxLength(50)
                   .IsRequired();

            // Índice único para UserName (no pueden haber dos iguales)
            builder.HasIndex(u => u.UserName)
                   .IsUnique();

            // Regla de Oro: El Email es la identidad única
            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.Email)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            // Auditoría y Teléfono (Nuevos)
            builder.Property(u => u.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(u => u.LastLogin)
                   .IsRequired(false);

            // Configuración de confirmación de correo
            builder.Property(u => u.EmailConfirmed)
                   .HasDefaultValue(false);

            builder.Property(u => u.EmailConfirmationToken)
                   .HasMaxLength(100);

            // Configuración de Refresh Token (Seguridad)
            builder.Property(u => u.RefreshToken)
                   .HasMaxLength(500);

            // Relación con Role
            builder.HasOne(u => u.Role)
                   .WithMany()
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}