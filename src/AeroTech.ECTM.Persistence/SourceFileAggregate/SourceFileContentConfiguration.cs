using AeroTech.ECTM.Domain.SourceFileAggregate.Entities;
using AeroTech.ECTM.Persistence._Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.SourceFileAggregate
{
    public sealed class SourceFileContentConfiguration : IEntityTypeConfiguration<SourceFileContent>
    {
        public void Configure(EntityTypeBuilder<SourceFileContent> builder)
        {
            builder.ToTable("SourceFileContents");
            builder.HasKey(content => content.Id);
            builder.Property(content => content.Id).ValueGeneratedNever();

            builder.Property(content => content.CompressedBytes)
                .HasColumnType("varbinary(max)")
                .IsRequired()
                .Metadata.SetValueComparer(ImmutableBytes.Comparer);
        }
    }
}
