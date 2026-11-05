using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightStabilityPointsAggregate
{
    public sealed class FlightStabilityPointsConfiguration : IEntityTypeConfiguration<FlightStabilityPoints>
    {
        public void Configure(EntityTypeBuilder<FlightStabilityPoints> builder)
        {
            builder.ToTable("FlightStabilityPoints");
            builder.HasKey(points => points.Id);
            builder.Property(points => points.Id).ValueGeneratedNever();

            builder.Property(points => points.FlightId).IsRequired();
            builder.Property(points => points.AnalysisRevisionId).IsRequired();
            builder.Property(points => points.SnapshotRulesDocumentId).IsRequired();
            builder.Property(points => points.SnapshotRulesVersion).IsRequired();
            builder.Property(points => points.Status).IsRequired();
            builder.Property(points => points.EvaluatedAt).IsRequired();
            builder.Property(points => points.Result).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(points => points.PhaseMetrics).HasColumnType("nvarchar(max)").IsRequired();

            builder.HasIndex(points => points.AnalysisRevisionId).IsUnique();
            builder.HasIndex(points => new { points.FlightId, points.Status });
        }
    }
}
