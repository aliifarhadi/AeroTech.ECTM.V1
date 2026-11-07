using AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.BaselinePrimarySelectionAggregate
{
    public sealed class BaselinePrimarySelectionConfiguration : IEntityTypeConfiguration<BaselinePrimarySelection>
    {
        public void Configure(EntityTypeBuilder<BaselinePrimarySelection> builder)
        {
            builder.ToTable("BaselinePrimarySelections");
            builder.HasKey(selection => selection.Id);
            builder.Property(selection => selection.Id).ValueGeneratedNever();

            builder.Property(selection => selection.EngineId).IsRequired();
            builder.Property(selection => selection.SnapshotKind).IsRequired();
            builder.Property(selection => selection.Selector).HasMaxLength(64);
            builder.Property(selection => selection.ParameterCode).HasMaxLength(64).IsRequired();
            builder.Property(selection => selection.BaselineId).IsRequired();
            builder.Property(selection => selection.BaselineParameterId).IsRequired();
            builder.Property(selection => selection.SelectedBy).HasMaxLength(200);
            builder.Property(selection => selection.SelectedAt).IsRequired();

            builder.HasIndex(selection => new { selection.EngineId, selection.SnapshotKind, selection.Selector, selection.ParameterCode })
                .IsUnique()
                .HasFilter(null);
            builder.HasIndex(selection => selection.BaselineId);
        }
    }
}
