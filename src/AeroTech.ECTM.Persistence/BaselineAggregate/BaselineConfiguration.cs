using AeroTech.ECTM.Domain.BaselineAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.BaselineAggregate
{
    public sealed class BaselineConfiguration : IEntityTypeConfiguration<Baseline>
    {
        public void Configure(EntityTypeBuilder<Baseline> builder)
        {
            builder.ToTable("Baselines");
            builder.HasKey(baseline => baseline.Id);
            builder.Property(baseline => baseline.Id).ValueGeneratedNever();

            builder.Property(baseline => baseline.EngineId).IsRequired();
            builder.Property(baseline => baseline.LineageId).IsRequired();
            builder.Property(baseline => baseline.RevisionNumber).IsRequired();
            builder.Property(baseline => baseline.State).IsRequired();
            builder.Property(baseline => baseline.DraftRevision).IsRequired();
            builder.Property(baseline => baseline.Name).HasMaxLength(200).IsRequired();
            builder.Property(baseline => baseline.SourceFileName).HasMaxLength(260);
            builder.Property(baseline => baseline.SourceFileSha256).HasMaxLength(64);
            builder.Property(baseline => baseline.Notes).HasMaxLength(2000);
            builder.Property(baseline => baseline.CreatedBy).HasMaxLength(200);
            builder.Property(baseline => baseline.CreatedAt).IsRequired();
            builder.Property(baseline => baseline.PublishedBy).HasMaxLength(200);
            builder.Property(baseline => baseline.RetiredBy).HasMaxLength(200);
            builder.Property(baseline => baseline.RetirementReason).HasMaxLength(2000);
            builder.Ignore(baseline => baseline.IsEvaluated);
            builder.Ignore(baseline => baseline.InputProvenance);

            builder.HasMany(baseline => baseline.Parameters)
                .WithOne()
                .HasForeignKey(parameter => parameter.BaselineId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(baseline => baseline.Parameters).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(baseline => new { baseline.LineageId, baseline.RevisionNumber }).IsUnique();
            builder.HasIndex(baseline => new { baseline.EngineId, baseline.State });
        }
    }
}
