namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed record ChannelCoverage(
        string ParameterKey,
        bool Recorded,
        int SampleCount,
        int ValuedCount,
        double Percent);
}
