using CC.Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CC.Infrastructure.Persistences.Contexts.Configurations
{
    public class SystemAuditConfiguration : IEntityTypeConfiguration<SystemAudit>
    {
        public void Configure(EntityTypeBuilder<SystemAudit> builder)
        {
            builder.ToTable("SystemAudits");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.RequestData).IsRequired(false);
            builder.Property(e => e.ErrorMessage).IsRequired(false);

            builder.HasIndex(e => e.CreatedAt);
            builder.HasIndex(e => e.UserId);

            builder.Ignore(e => e.AuditCreateDate);
            builder.Ignore(e => e.AuditUpdateDate);
            builder.Ignore(e => e.AuditDeleteDate);
        }
    }
}
