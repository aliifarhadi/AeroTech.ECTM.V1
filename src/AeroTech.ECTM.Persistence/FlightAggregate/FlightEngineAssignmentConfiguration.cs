using AeroTech.ECTM.Domain.FlightAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightAggregate
{
    public sealed class FlightEngineAssignmentConfiguration : IEntityTypeConfiguration<FlightEngineAssignment>
    {
        public void Configure(EntityTypeBuilder<FlightEngineAssignment> builder)
        {
            builder.ToTable("FlightEngineAssignments", table =>
                table.HasCheckConstraint("CK_FlightEngineAssignments_Position", "[Position] IN (1, 2)"));
            builder.HasKey(engine => engine.Id);
            builder.Property(engine => engine.Id).ValueGeneratedNever();

            builder.Property(engine => engine.Position).IsRequired();
            builder.Property(engine => engine.Status).IsRequired();

            builder.HasIndex(engine => new { engine.FlightId, engine.Position }).IsUnique();
            builder.HasIndex(engine => engine.EngineId);
        }
    }
}
