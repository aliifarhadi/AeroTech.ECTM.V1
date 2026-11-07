using AeroTech.ECTM.Domain.BaselineAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.BaselineAggregate
{
    public sealed class BaselineParameterConfiguration : IEntityTypeConfiguration<BaselineParameter>
    {
        public void Configure(EntityTypeBuilder<BaselineParameter> builder)
        {
            builder.ToTable("BaselineParameters");
            builder.HasKey(parameter => parameter.Id);
            builder.Property(parameter => parameter.Id).ValueGeneratedNever();

            builder.Property(parameter => parameter.SnapshotKind).IsRequired();
            builder.Property(parameter => parameter.Selector).HasMaxLength(64);
            builder.Property(parameter => parameter.Statistic).IsRequired();
            builder.Property(parameter => parameter.ParameterCode).HasMaxLength(64).IsRequired();
            builder.Property(parameter => parameter.Unit).HasMaxLength(64);
            builder.Property(parameter => parameter.ValueOrigin).IsRequired();
            builder.Property(parameter => parameter.ReferenceContext).HasColumnType("nvarchar(max)");
            builder.Property(parameter => parameter.CatalogDocumentId).IsRequired();
            builder.Property(parameter => parameter.CatalogVersion).IsRequired();
            builder.Property(parameter => parameter.SnapshotRulesDocumentId).IsRequired();
            builder.Property(parameter => parameter.SnapshotRulesVersion).IsRequired();
            builder.Property(parameter => parameter.Notes).HasMaxLength(2000);

            builder.HasIndex(parameter => new { parameter.BaselineId, parameter.SnapshotKind, parameter.Selector, parameter.ParameterCode })
                .IsUnique()
                .HasFilter(null);
        }
    }
}
