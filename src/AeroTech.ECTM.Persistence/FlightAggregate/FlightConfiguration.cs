using AeroTech.ECTM.Domain.FlightAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightAggregate
{
    public sealed class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.ToTable("Flights");
            builder.HasKey(flight => flight.Id);
            builder.Property(flight => flight.Id).ValueGeneratedNever();

            builder.Property(flight => flight.SourceFileId).IsRequired();
            builder.Property(flight => flight.ImportBatchId).IsRequired();
            builder.Property(flight => flight.ImportItemId).IsRequired();
            builder.Property(flight => flight.Registration).HasMaxLength(64);
            builder.Property(flight => flight.FlightNumber).HasMaxLength(32);
            builder.Property(flight => flight.IdentityStatus).IsRequired();
            builder.PrimitiveCollection(flight => flight.IdentityReasons).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(flight => flight.IdentityNote).HasMaxLength(2000);
            builder.Property(flight => flight.State).IsRequired();
            builder.Property(flight => flight.HoldNote).HasMaxLength(2000);
            builder.Property(flight => flight.DataProfileDocumentId).IsRequired();
            builder.Property(flight => flight.DataProfileVersion).IsRequired();
            builder.Property(flight => flight.ParameterCatalogDocumentId).IsRequired();
            builder.Property(flight => flight.ParameterCatalogVersion).IsRequired();
            builder.Property(flight => flight.TimeParameterKey).HasMaxLength(64);
            builder.Property(flight => flight.SampleCount).IsRequired();
            builder.Property(flight => flight.ImportedAt).IsRequired();
            builder.Ignore(flight => flight.Positions);
            builder.Ignore(flight => flight.Versions);

            builder.HasMany(flight => flight.Engines)
                .WithOne()
                .HasForeignKey(engine => engine.FlightId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(flight => flight.Engines).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(flight => flight.Issues)
                .WithOne()
                .HasForeignKey(issue => issue.FlightId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(flight => flight.Issues).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(flight => flight.Channels)
                .WithOne()
                .HasForeignKey(channel => channel.FlightId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(flight => flight.Channels).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(flight => flight.SourceFileId).IsUnique();
            builder.HasIndex(flight => new { flight.AircraftId, flight.FlightDate, flight.FlightNumber });
            builder.HasIndex(flight => flight.ImportBatchId);
            builder.HasIndex(flight => flight.State);
        }
    }
}
