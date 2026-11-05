using AeroTech.ECTM.Domain.ImportBatchAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.ImportBatchAggregate
{
    public sealed class ImportBatchConfiguration : IEntityTypeConfiguration<ImportBatch>
    {
        public void Configure(EntityTypeBuilder<ImportBatch> builder)
        {
            builder.ToTable("ImportBatches");
            builder.HasKey(batch => batch.Id);
            builder.Property(batch => batch.Id).ValueGeneratedNever();

            builder.Property(batch => batch.State).IsRequired();
            builder.Property(batch => batch.DataProfileDocumentId).IsRequired();
            builder.Property(batch => batch.DataProfileVersion).IsRequired();
            builder.Property(batch => batch.ParameterCatalogDocumentId).IsRequired();
            builder.Property(batch => batch.ParameterCatalogVersion).IsRequired();
            builder.Property(batch => batch.DefaultRegistration).HasMaxLength(64);
            builder.Property(batch => batch.IdempotencyKey).HasMaxLength(128);
            builder.Property(batch => batch.CreatedAt).IsRequired();
            builder.Ignore(batch => batch.Versions);

            builder.HasMany(batch => batch.Items)
                .WithOne()
                .HasForeignKey(item => item.ImportBatchId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(batch => batch.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(batch => batch.CreatedAt);
        }
    }
}
