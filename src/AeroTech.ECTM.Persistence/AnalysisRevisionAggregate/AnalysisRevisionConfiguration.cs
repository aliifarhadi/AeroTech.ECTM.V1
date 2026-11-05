using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.AnalysisRevisionAggregate
{
    public sealed class AnalysisRevisionConfiguration : IEntityTypeConfiguration<AnalysisRevision>
    {
        public void Configure(EntityTypeBuilder<AnalysisRevision> builder)
        {
            builder.ToTable("AnalysisRevisions");
            builder.HasKey(revision => revision.Id);
            builder.Property(revision => revision.Id).ValueGeneratedNever();

            builder.Property(revision => revision.FlightId).IsRequired();
            builder.Property(revision => revision.Number).IsRequired();
            builder.Property(revision => revision.Trigger).IsRequired();
            builder.Property(revision => revision.Status).IsRequired();
            builder.Property(revision => revision.SourceSha256).HasMaxLength(64).IsRequired();
            builder.Property(revision => revision.DataProfileDocumentId).IsRequired();
            builder.Property(revision => revision.DataProfileVersion).IsRequired();
            builder.Property(revision => revision.ParameterCatalogDocumentId).IsRequired();
            builder.Property(revision => revision.ParameterCatalogVersion).IsRequired();
            builder.Property(revision => revision.QualityRulesDocumentId).IsRequired();
            builder.Property(revision => revision.QualityRulesVersion).IsRequired();
            builder.Property(revision => revision.EngineAssignments).HasMaxLength(200).IsRequired();
            builder.Property(revision => revision.FailureCode).HasMaxLength(100);
            builder.Property(revision => revision.CreatedAt).IsRequired();

            builder.HasIndex(revision => new { revision.FlightId, revision.Number }).IsUnique();
            builder.HasIndex(revision => new { revision.FlightId, revision.Status });
        }
    }
}
