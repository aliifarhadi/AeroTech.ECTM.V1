using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.ProcessingJobAggregate
{
    public sealed class ProcessingJobConfiguration : IEntityTypeConfiguration<ProcessingJob>
    {
        public void Configure(EntityTypeBuilder<ProcessingJob> builder)
        {
            builder.ToTable("ProcessingJobs");
            builder.HasKey(job => job.Id);
            builder.Property(job => job.Id).ValueGeneratedNever();

            builder.Property(job => job.Kind).IsRequired();
            builder.Property(job => job.SubjectId).IsRequired();
            builder.Property(job => job.State).IsRequired();
            builder.Property(job => job.Processed).IsRequired();
            builder.Property(job => job.Total).IsRequired();
            builder.Property(job => job.ErrorCode).HasMaxLength(64);
            builder.Property(job => job.CreatedAt).IsRequired();

            builder.HasIndex(job => new { job.State, job.CreatedAt });
        }
    }
}
