using AeroTech.ECTM.Domain.ImportBatchAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.ImportBatchAggregate
{
    public sealed class ImportItemConfiguration : IEntityTypeConfiguration<ImportItem>
    {
        public void Configure(EntityTypeBuilder<ImportItem> builder)
        {
            builder.ToTable("ImportItems");
            builder.HasKey(item => item.Id);
            builder.Property(item => item.Id).ValueGeneratedNever();

            builder.Property(item => item.Ordinal).IsRequired();
            builder.Property(item => item.FileName).HasMaxLength(256).IsRequired();
            builder.Property(item => item.ContainerName).HasMaxLength(256);
            builder.Property(item => item.SizeBytes).IsRequired();
            builder.Property(item => item.Sha256).HasMaxLength(64);
            builder.Property(item => item.Outcome).IsRequired();
            builder.Property(item => item.RejectionCode).HasMaxLength(64);
            builder.Property(item => item.State).IsRequired();
            builder.Property(item => item.ErrorCode).HasMaxLength(64);
            builder.Property(item => item.Preview).HasColumnType("nvarchar(max)").IsRequired();
            builder.Ignore(item => item.IsPending);

            builder.HasIndex(item => new { item.ImportBatchId, item.Ordinal }).IsUnique();
            builder.HasIndex(item => item.SourceFileId);
            builder.HasIndex(item => item.FlightId);
        }
    }
}
