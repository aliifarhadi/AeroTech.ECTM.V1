using AeroTech.ECTM.Domain._Shared;
using AeroTech.Framework.Core.Domain.Entities;

namespace AeroTech.ECTM.Domain.FlightAggregate.Entities
{
    public sealed class FlightChannelPayload : Entity<long>, IBulkData
    {
        private FlightChannelPayload()
        {
        }

        internal FlightChannelPayload(long channelId, byte[] bytes)
        {
            Id = channelId;
            Bytes = bytes;
        }

        public byte[] Bytes { get; private set; } = default!;
    }
}
