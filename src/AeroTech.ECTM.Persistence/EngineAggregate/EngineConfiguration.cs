using AeroTech.ECTM.Domain.EngineAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.EngineAggregate
{
    public sealed class EngineConfiguration : IEntityTypeConfiguration<Engine>
    {
        public void Configure(EntityTypeBuilder<Engine> builder)
        {
            builder.ToTable("Engines");
            builder.HasKey(engine => engine.Id);
            builder.Property(engine => engine.Id).ValueGeneratedNever();

            builder.Property(engine => engine.EngineModelId).IsRequired();
            builder.Property(engine => engine.Manufacturer).HasMaxLength(128).IsRequired();
            builder.Property(engine => engine.SerialNumber)
                .HasMaxLength(64)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .IsRequired();
            builder.Property(engine => engine.Status).IsRequired();
            builder.Property(engine => engine.VariantCode).HasMaxLength(64);
            builder.Property(engine => engine.ThrustRatingCode).HasMaxLength(64);
            builder.Property(engine => engine.ConfigurationEvidenceReference).HasMaxLength(256);
            builder.Property(engine => engine.ConfigurationDocumentedBy).HasMaxLength(256);
            builder.Property(engine => engine.HoursSinceNew).HasPrecision(12, 2);
            builder.Property(engine => engine.HoursSinceLastEvent).HasPrecision(12, 2);

            builder.HasMany(engine => engine.LifeCounterAdjustments)
                .WithOne()
                .HasForeignKey(adjustment => adjustment.EngineId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(engine => engine.LifeCounterAdjustments).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(engine => new { engine.Manufacturer, engine.SerialNumber }).IsUnique();
            builder.HasIndex(engine => engine.EngineModelId);
        }
    }
}
