using AeroTech.ECTM.Domain.FlightReviewAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightReviewAggregate
{
    public sealed class FlightReviewEntryConfiguration : IEntityTypeConfiguration<FlightReviewEntry>
    {
        public void Configure(EntityTypeBuilder<FlightReviewEntry> builder)
        {
            builder.ToTable("FlightReviewEntries");
            builder.HasKey(entry => entry.Id);
            builder.Property(entry => entry.Id).ValueGeneratedNever();

            builder.Property(entry => entry.Action).IsRequired();
            builder.Property(entry => entry.Actor).HasMaxLength(200);
            builder.Property(entry => entry.Note).HasMaxLength(2000);
            builder.Property(entry => entry.FlightStateAfter).IsRequired();
            builder.Property(entry => entry.At).IsRequired();

            builder.HasIndex(entry => entry.At);
        }
    }
}
