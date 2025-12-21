using CC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistences.Contexts.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique(); // Regla de oro: Nombres únicos
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ShowName).HasMaxLength(100).IsRequired();
        }
    }
}
