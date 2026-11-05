namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record SeriesPoint(int? SampleIndex, double Elapsed, double? Value);
}
