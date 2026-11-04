using AeroTech.ECTM.Domain.AuditEntryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.AuditEntryAggregate
{
    public sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
    {
        public void Configure(EntityTypeBuilder<AuditEntry> builder)
        {
            builder.ToTable("AuditEntries");
            builder.HasKey(entry => entry.Id);
            builder.Property(entry => entry.Id).ValueGeneratedNever();

            builder.Property(entry => entry.OccurredAt).IsRequired();
            builder.Property(entry => entry.ActorSubject).HasMaxLength(256);
            builder.Property(entry => entry.EntityType).HasMaxLength(128).IsRequired();
            builder.Property(entry => entry.EntityId).IsRequired();
            builder.Property(entry => entry.Action).IsRequired();
            builder.Property(entry => entry.Before).HasColumnType("nvarchar(max)");
            builder.Property(entry => entry.After).HasColumnType("nvarchar(max)");
            builder.Property(entry => entry.CorrelationId).HasMaxLength(32);

            builder.HasIndex(entry => new { entry.EntityType, entry.EntityId });
            builder.HasIndex(entry => entry.OccurredAt);
        }
    }
}
