using AeroTech.ECTM.Domain.FlightAggregate.Entities;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeFlightChannelDto(
        string ParameterKey,
        string ParameterCode,
        int? Position,
        string Unit,
        string ValueKind,
        int ProfileOrdinal,
        int SampleCount,
        int MissingCount,
        int UnparseableCount,
        int OutOfRangeCount,
        string PayloadSha256)
    {
        public static BackofficeFlightChannelDto From(FlightChannel channel)
            => new(
                channel.ParameterKey,
                channel.ParameterCode,
                channel.Position,
                channel.Unit,
                channel.ValueKind,
                channel.ProfileOrdinal,
                channel.SampleCount,
                channel.MissingCount,
                channel.UnparseableCount,
                channel.OutOfRangeCount,
                channel.PayloadSha256);
    }
}
