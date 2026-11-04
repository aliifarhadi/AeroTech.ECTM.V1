using AeroTech.ECTM.Domain.EngineInstallationAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.EngineInstallationAggregate
{
    public sealed class RegistryChangeConfiguration : IEntityTypeConfiguration<RegistryChange>
    {
        public void Configure(EntityTypeBuilder<RegistryChange> builder)
        {
            builder.ToTable("RegistryChanges");
            builder.HasKey(change => change.Id);
            builder.Property(change => change.Id).ValueGeneratedNever();

            builder.Property(change => change.RegistryVersion)
                .HasDefaultValueSql($"NEXT VALUE FOR [{EctmDbContext.DefaultSchema}].[{EctmDbContext.RegistryVersionSequence}]")
                .ValueGeneratedOnAdd();
            builder.Property(change => change.ChangeType).IsRequired();
            builder.Property(change => change.Reason).HasMaxLength(256).IsRequired();
            builder.Property(change => change.ChangedAt).IsRequired();

            builder.HasIndex(change => change.RegistryVersion).IsUnique();
        }
    }
}
