using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightPhaseAssessmentAggregate
{
    public sealed class FlightPhaseAssessmentConfiguration : IEntityTypeConfiguration<FlightPhaseAssessment>
    {
        public void Configure(EntityTypeBuilder<FlightPhaseAssessment> builder)
        {
            builder.ToTable("FlightPhaseAssessments");
            builder.HasKey(assessment => assessment.Id);
            builder.Property(assessment => assessment.Id).ValueGeneratedNever();

            builder.Property(assessment => assessment.FlightId).IsRequired();
            builder.Property(assessment => assessment.AnalysisRevisionId).IsRequired();
            builder.Property(assessment => assessment.PhaseRuleSetDocumentId).IsRequired();
            builder.Property(assessment => assessment.PhaseRuleSetVersion).IsRequired();
            builder.Property(assessment => assessment.Status).IsRequired();
            builder.Property(assessment => assessment.ReferenceAlgorithm).HasMaxLength(100).IsRequired();
            builder.Property(assessment => assessment.ReferenceMatch).IsRequired();
            builder.Property(assessment => assessment.NotEvaluableReason).HasMaxLength(100);
            builder.Property(assessment => assessment.SegmentCount).IsRequired();
            builder.Property(assessment => assessment.UnknownSampleCount).IsRequired();
            builder.Property(assessment => assessment.EventSpanCount).IsRequired();
            builder.Property(assessment => assessment.EvaluatedAt).IsRequired();
            builder.Property(assessment => assessment.Result).HasColumnType("nvarchar(max)").IsRequired();

            builder.HasIndex(assessment => assessment.AnalysisRevisionId).IsUnique();
            builder.HasIndex(assessment => new { assessment.FlightId, assessment.Status });
        }
    }
}
