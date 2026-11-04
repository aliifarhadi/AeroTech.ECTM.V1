using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.ConfigurationDocumentAggregate
{
    public sealed class ConfigurationDocumentConfiguration : IEntityTypeConfiguration<ConfigurationDocument>
    {
        public void Configure(EntityTypeBuilder<ConfigurationDocument> builder)
        {
            builder.ToTable("ConfigurationDocuments");
            builder.HasKey(document => document.Id);
            builder.Property(document => document.Id).ValueGeneratedNever();

            builder.Property(document => document.Kind).IsRequired();
            builder.Property(document => document.VersionNumber).IsRequired();
            builder.Property(document => document.Status).IsRequired();
            builder.Property(document => document.BasedOnDocumentId);
            builder.Property(document => document.Content).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(document => document.ContentSha256).HasMaxLength(64).IsRequired();
            builder.Property(document => document.PublishedBy).HasMaxLength(256);
            builder.Property(document => document.ApprovalNote).HasMaxLength(2000);
            builder.Property(document => document.RetiredBy).HasMaxLength(256);
            builder.Property(document => document.RetirementNote).HasMaxLength(2000);

            builder.HasIndex(document => new { document.Kind, document.VersionNumber }).IsUnique();
        }
    }
}
