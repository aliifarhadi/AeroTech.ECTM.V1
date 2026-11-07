using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.BaselineParameterFlightAggregate
{
    public sealed class BaselineParameterFlightConfiguration : IEntityTypeConfiguration<BaselineParameterFlight>
    {
        public void Configure(EntityTypeBuilder<BaselineParameterFlight> builder)
        {
            builder.ToTable("BaselineParameterFlights");
            builder.HasKey(evaluation => evaluation.Id);
            builder.Property(evaluation => evaluation.Id).ValueGeneratedNever();

            builder.Property(evaluation => evaluation.BaselineId).IsRequired();
            builder.Property(evaluation => evaluation.BaselineParameterId).IsRequired();
            builder.Property(evaluation => evaluation.EngineId).IsRequired();
            builder.Property(evaluation => evaluation.FlightId).IsRequired();
            builder.Property(evaluation => evaluation.AnalysisRevisionId).IsRequired();
            builder.Property(evaluation => evaluation.FlightStabilityPointsId).IsRequired();
            builder.Property(evaluation => evaluation.Position).IsRequired();
            builder.Property(evaluation => evaluation.SnapshotKind).IsRequired();
            builder.Property(evaluation => evaluation.Selector).HasMaxLength(64);
            builder.Property(evaluation => evaluation.Statistic).IsRequired();
            builder.Property(evaluation => evaluation.ParameterCode).HasMaxLength(64).IsRequired();
            builder.Property(evaluation => evaluation.Unit).HasMaxLength(64);
            builder.Property(evaluation => evaluation.SnapshotRulesDocumentId).IsRequired();
            builder.Property(evaluation => evaluation.SnapshotRulesVersion).IsRequired();
            builder.Property(evaluation => evaluation.BaselineRawValue).IsRequired();
            builder.Property(evaluation => evaluation.Status).IsRequired();
            builder.PrimitiveCollection(evaluation => evaluation.Reasons).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(evaluation => evaluation.Detail).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(evaluation => evaluation.Fingerprint).HasMaxLength(64).IsRequired();
            builder.Property(evaluation => evaluation.CalculationVersion).IsRequired();
            builder.Property(evaluation => evaluation.IsCurrent).IsRequired();
            builder.Property(evaluation => evaluation.CalculatedAt).IsRequired();

            builder.HasIndex(evaluation => new { evaluation.BaselineParameterId, evaluation.FlightId })
                .IsUnique()
                .HasFilter("[IsCurrent] = 1");
            builder.HasIndex(evaluation => new { evaluation.EngineId, evaluation.FlightDate });
            builder.HasIndex(evaluation => new { evaluation.FlightId, evaluation.IsCurrent });
        }
    }
}
