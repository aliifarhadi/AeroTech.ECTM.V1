using AeroTech.ECTM.Domain.FlightReviewAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightReviewAggregate
{
    public sealed class FlightReviewConfiguration : IEntityTypeConfiguration<FlightReview>
    {
        public void Configure(EntityTypeBuilder<FlightReview> builder)
        {
            builder.ToTable("FlightReviews");
            builder.HasKey(review => review.Id);
            builder.Property(review => review.Id).ValueGeneratedNever();

            builder.Property(review => review.FlightId).IsRequired();
            builder.Property(review => review.AnalysisRevisionId).IsRequired();
            builder.Property(review => review.State).IsRequired();
            builder.Property(review => review.DecidedBy).HasMaxLength(200);
            builder.Property(review => review.Note).HasMaxLength(2000);
            builder.Property(review => review.OpenedAt).IsRequired();

            builder.HasMany(review => review.Entries)
                .WithOne()
                .HasForeignKey(entry => entry.FlightReviewId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(review => review.Entries).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(review => review.AnalysisRevisionId).IsUnique();
            builder.HasIndex(review => new { review.FlightId, review.State });
        }
    }
}
