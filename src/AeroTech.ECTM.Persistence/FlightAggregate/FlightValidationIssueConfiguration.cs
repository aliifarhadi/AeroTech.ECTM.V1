using AeroTech.ECTM.Domain.FlightAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightAggregate
{
    public sealed class FlightValidationIssueConfiguration : IEntityTypeConfiguration<FlightValidationIssue>
    {
        public void Configure(EntityTypeBuilder<FlightValidationIssue> builder)
        {
            builder.ToTable("FlightValidationIssues");
            builder.HasKey(issue => issue.Id);
            builder.Property(issue => issue.Id).ValueGeneratedNever();

            builder.Property(issue => issue.Severity).IsRequired();
            builder.Property(issue => issue.Code).HasMaxLength(64).IsRequired();
            builder.Property(issue => issue.Message).HasMaxLength(4000).IsRequired();
            builder.Property(issue => issue.ParameterKey).HasMaxLength(64);
        }
    }
}
