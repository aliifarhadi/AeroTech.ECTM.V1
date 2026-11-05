using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Entities
{
    public sealed class FlightChannel : Entity<long>, IBulkData
    {
        private FlightChannel()
        {
        }

        private FlightChannel(long id, long flightId, RecordedChannel channel, ChannelEncoding encoding, EncodedChannel encoded)
        {
            Id = id;
            FlightId = flightId;
            ParameterKey = channel.ParameterKey;
            ParameterCode = channel.ParameterCode;
            Position = (byte?)channel.Position;
            ProfileOrdinal = channel.Ordinal;
            Unit = channel.Unit;
            ValueKind = channel.ValueKind;
            Encoding = encoding;
            SampleCount = channel.SampleCount;
            MissingCount = channel.MissingCount;
            UnparseableCount = channel.UnparseableCount;
            OutOfRangeCount = channel.OutOfRangeCount;
            PayloadSha256 = encoded.Sha256;
            Payload = new FlightChannelPayload(id, encoded.Payload);
        }

        public long FlightId { get; private set; }

        public string ParameterKey { get; private set; } = default!;

        public string ParameterCode { get; private set; } = default!;

        public byte? Position { get; private set; }

        public int ProfileOrdinal { get; private set; }

        public string Unit { get; private set; } = default!;

        public string ValueKind { get; private set; } = default!;

        public ChannelEncoding Encoding { get; private set; }

        public int SampleCount { get; private set; }

        public int MissingCount { get; private set; }

        public int UnparseableCount { get; private set; }

        public int OutOfRangeCount { get; private set; }

        public string PayloadSha256 { get; private set; } = default!;

        public FlightChannelPayload? Payload { get; private set; }

        public bool IsNumeric => Encoding == ChannelEncoding.FLOAT64_GZIP;

        internal static FlightChannel Record(long id, long flightId, RecordedChannel channel)
        {
            if (channel.IsNumeric)
                return new FlightChannel(id, flightId, channel, ChannelEncoding.FLOAT64_GZIP, ChannelCodec.EncodeNumbers(channel.Numbers!));

            return new FlightChannel(id, flightId, channel, ChannelEncoding.TEXT_JSON_GZIP, ChannelCodec.EncodeTexts(channel.Texts!));
        }

        public double?[] NumbersFrom(FlightChannelPayload payload) => ChannelCodec.DecodeNumbers(payload.Bytes);

        public string?[] TextsFrom(FlightChannelPayload payload) => ChannelCodec.DecodeTexts(payload.Bytes);
    }
}
