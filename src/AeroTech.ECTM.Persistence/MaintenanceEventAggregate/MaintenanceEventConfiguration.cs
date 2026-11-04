using AeroTech.ECTM.Domain.MaintenanceEventAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.MaintenanceEventAggregate
{
    public sealed class MaintenanceEventConfiguration : IEntityTypeConfiguration<MaintenanceEvent>
    {
        private const string EngineIdentity = "([IdentityKind] = 1 AND [EngineId] IS NOT NULL AND [AircraftId] IS NULL AND [Position] IS NULL)";
        private const string PositionIdentity = "([IdentityKind] = 2 AND [EngineId] IS NULL AND [AircraftId] IS NOT NULL AND [Position] IN (1, 2))";

        public void Configure(EntityTypeBuilder<MaintenanceEvent> builder)
        {
            builder.ToTable("MaintenanceEvents", table =>
                table.HasCheckConstraint("CK_MaintenanceEvents_Identity", $"{EngineIdentity} OR {PositionIdentity}"));
            builder.HasKey(maintenanceEvent => maintenanceEvent.Id);
            builder.Property(maintenanceEvent => maintenanceEvent.Id).ValueGeneratedNever();

            builder.Property(maintenanceEvent => maintenanceEvent.IdentityKind).IsRequired();
            builder.Property(maintenanceEvent => maintenanceEvent.EventType).IsRequired();
            builder.Property(maintenanceEvent => maintenanceEvent.EventAt).IsRequired();
            builder.Property(maintenanceEvent => maintenanceEvent.DateReliability).IsRequired();
            builder.Property(maintenanceEvent => maintenanceEvent.HoursAtEvent).HasPrecision(12, 2);
            builder.Property(maintenanceEvent => maintenanceEvent.WorkOrderReference).HasMaxLength(128);
            builder.Property(maintenanceEvent => maintenanceEvent.Notes).HasMaxLength(2000);

            builder.HasIndex(maintenanceEvent => maintenanceEvent.EngineId);
            builder.HasIndex(maintenanceEvent => new { maintenanceEvent.AircraftId, maintenanceEvent.Position });
        }
    }
}
