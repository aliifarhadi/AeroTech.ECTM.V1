using AeroTech.ECTM.Domain.FlightAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightAggregate
{
    public sealed class FlightChannelConfiguration : IEntityTypeConfiguration<FlightChannel>
    {
        public void Configure(EntityTypeBuilder<FlightChannel> builder)
        {
            builder.ToTable("FlightChannels");
            builder.HasKey(channel => channel.Id);
            builder.Property(channel => channel.Id).ValueGeneratedNever();

            builder.Property(channel => channel.ParameterKey).HasMaxLength(64).IsRequired();
            builder.Property(channel => channel.ParameterCode).HasMaxLength(64).IsRequired();
            builder.Property(channel => channel.ProfileOrdinal).IsRequired();
            builder.Property(channel => channel.Unit).HasMaxLength(64).IsRequired();
            builder.Property(channel => channel.ValueKind).HasMaxLength(16).IsRequired();
            builder.Property(channel => channel.Encoding).IsRequired();
            builder.Property(channel => channel.SampleCount).IsRequired();
            builder.Property(channel => channel.PayloadSha256).HasMaxLength(64).IsRequired();
            builder.Ignore(channel => channel.IsNumeric);

            builder.HasOne(channel => channel.Payload)
                .WithOne()
                .HasForeignKey<FlightChannelPayload>(payload => payload.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(channel => new { channel.FlightId, channel.ParameterKey }).IsUnique();
        }
    }
}
