using AeroTech.ECTM.Domain.FlightQualityAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightQualityAggregate
{
    public sealed class FlightQualityConfiguration : IEntityTypeConfiguration<FlightQuality>
    {
        public void Configure(EntityTypeBuilder<FlightQuality> builder)
        {
            builder.ToTable("FlightQualities");
            builder.HasKey(quality => quality.Id);
            builder.Property(quality => quality.Id).ValueGeneratedNever();

            builder.Property(quality => quality.FlightId).IsRequired();
            builder.Property(quality => quality.AnalysisRevisionId).IsRequired();
            builder.Property(quality => quality.QualityRulesDocumentId).IsRequired();
            builder.Property(quality => quality.QualityRulesVersion).IsRequired();
            builder.Property(quality => quality.Status).IsRequired();
            builder.Property(quality => quality.Grade).IsRequired();
            builder.Property(quality => quality.DiscontinuityCount).IsRequired();
            builder.Property(quality => quality.SpanCount).IsRequired();
            builder.Property(quality => quality.EvaluatedAt).IsRequired();
            builder.Property(quality => quality.Result).HasColumnType("nvarchar(max)").IsRequired();

            builder.HasIndex(quality => quality.AnalysisRevisionId).IsUnique();
            builder.HasIndex(quality => new { quality.FlightId, quality.Status });
        }
    }
}
