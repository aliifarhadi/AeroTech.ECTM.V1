using AeroTech.ECTM.Domain.FlightAggregate.Entities;
using AeroTech.ECTM.Persistence._Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.FlightAggregate
{
    public sealed class FlightChannelPayloadConfiguration : IEntityTypeConfiguration<FlightChannelPayload>
    {
        public void Configure(EntityTypeBuilder<FlightChannelPayload> builder)
        {
            builder.ToTable("FlightChannelPayloads");
            builder.HasKey(payload => payload.Id);
            builder.Property(payload => payload.Id).ValueGeneratedNever();

            builder.Property(payload => payload.Bytes)
                .HasColumnType("varbinary(max)")
                .IsRequired()
                .Metadata.SetValueComparer(ImmutableBytes.Comparer);
        }
    }
}
