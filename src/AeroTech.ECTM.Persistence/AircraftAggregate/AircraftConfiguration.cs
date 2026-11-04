using AeroTech.ECTM.Domain.AircraftAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.AircraftAggregate
{
    public sealed class AircraftConfiguration : IEntityTypeConfiguration<Aircraft>
    {
        public void Configure(EntityTypeBuilder<Aircraft> builder)
        {
            builder.ToTable("Aircraft");
            builder.HasKey(aircraft => aircraft.Id);
            builder.Property(aircraft => aircraft.Id).ValueGeneratedNever();

            builder.Property(aircraft => aircraft.Registration)
                .HasMaxLength(16)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .IsRequired();
            builder.Property(aircraft => aircraft.TypeCode).HasMaxLength(64);
            builder.Property(aircraft => aircraft.OperatorCode).HasMaxLength(64);
            builder.Property(aircraft => aircraft.EngineCount).IsRequired();
            builder.Property(aircraft => aircraft.Status).IsRequired();

            builder.HasIndex(aircraft => aircraft.Registration).IsUnique();
        }
    }
}
