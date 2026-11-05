using AeroTech.ECTM.Domain.SourceFileAggregate;
using AeroTech.ECTM.Domain.SourceFileAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.SourceFileAggregate
{
    public sealed class SourceFileConfiguration : IEntityTypeConfiguration<SourceFile>
    {
        public void Configure(EntityTypeBuilder<SourceFile> builder)
        {
            builder.ToTable("SourceFiles");
            builder.HasKey(sourceFile => sourceFile.Id);
            builder.Property(sourceFile => sourceFile.Id).ValueGeneratedNever();

            builder.Property(sourceFile => sourceFile.FileName).HasMaxLength(256).IsRequired();
            builder.Property(sourceFile => sourceFile.Sha256).HasMaxLength(64).IsRequired();
            builder.Property(sourceFile => sourceFile.SizeBytes).IsRequired();
            builder.Property(sourceFile => sourceFile.SourceType).IsRequired();
            builder.Property(sourceFile => sourceFile.StoredAt).IsRequired();

            builder.HasOne(sourceFile => sourceFile.Content)
                .WithOne()
                .HasForeignKey<SourceFileContent>(content => content.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(sourceFile => sourceFile.Sha256).IsUnique();
        }
    }
}
