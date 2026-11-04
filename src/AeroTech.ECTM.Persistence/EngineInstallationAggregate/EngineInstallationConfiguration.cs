using AeroTech.ECTM.Domain.EngineInstallationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.EngineInstallationAggregate
{
    public sealed class EngineInstallationConfiguration : IEntityTypeConfiguration<EngineInstallation>
    {
        public void Configure(EntityTypeBuilder<EngineInstallation> builder)
        {
            builder.ToTable("EngineInstallations", table =>
            {
                table.HasCheckConstraint("CK_EngineInstallations_Position", "[Position] IN (1, 2)");
                table.HasCheckConstraint("CK_EngineInstallations_Interval", "[RemovedAt] IS NULL OR [RemovedAt] > [InstalledAt]");
            });
            builder.HasKey(installation => installation.Id);
            builder.Property(installation => installation.Id).ValueGeneratedNever();

            builder.Property(installation => installation.EngineId).IsRequired();
            builder.Property(installation => installation.AircraftId).IsRequired();
            builder.Property(installation => installation.Position).IsRequired();
            builder.Property(installation => installation.InstalledAt).IsRequired();
            builder.Property(installation => installation.Reason).HasMaxLength(256).IsRequired();
            builder.Property(installation => installation.Evidence).HasMaxLength(256);
            builder.Property(installation => installation.RemovalReason).HasMaxLength(256);
            builder.Ignore(installation => installation.IsOpen);

            builder.HasMany(installation => installation.Changes)
                .WithOne()
                .HasForeignKey(change => change.InstallationId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(installation => installation.Changes).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(installation => new { installation.AircraftId, installation.Position, installation.InstalledAt }).IsUnique();
            builder.HasIndex(installation => new { installation.EngineId, installation.InstalledAt }).IsUnique();
            builder.HasIndex(installation => installation.EngineId)
                .IsUnique()
                .HasFilter("[RemovedAt] IS NULL");
            builder.HasIndex(installation => new { installation.AircraftId, installation.Position })
                .IsUnique()
                .HasFilter("[RemovedAt] IS NULL");
        }
    }
}
