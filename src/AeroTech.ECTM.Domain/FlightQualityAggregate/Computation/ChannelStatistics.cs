namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed record ChannelStatistics(
        string ParameterKey,
        string ParameterCode,
        int ProfileOrdinal,
        int SampleCount,
        int ValuedCount,
        int OutOfRangeCount);
}
