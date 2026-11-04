using AeroTech.ECTM.Domain.EngineAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.EngineAggregate
{
    public sealed class LifeCounterAdjustmentConfiguration : IEntityTypeConfiguration<LifeCounterAdjustment>
    {
        public void Configure(EntityTypeBuilder<LifeCounterAdjustment> builder)
        {
            builder.ToTable("EngineLifeCounterAdjustments");
            builder.HasKey(adjustment => adjustment.Id);
            builder.Property(adjustment => adjustment.Id).ValueGeneratedNever();

            builder.Property(adjustment => adjustment.HoursSinceNew).HasPrecision(12, 2);
            builder.Property(adjustment => adjustment.HoursSinceLastEvent).HasPrecision(12, 2);
            builder.Property(adjustment => adjustment.BasisAt).IsRequired();
            builder.Property(adjustment => adjustment.Source).IsRequired();
            builder.Property(adjustment => adjustment.Reason).HasMaxLength(256).IsRequired();
            builder.Property(adjustment => adjustment.AdjustedAt).IsRequired();
            builder.Property(adjustment => adjustment.AdjustedBy).HasMaxLength(256);
        }
    }
}
