namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record SampledSeries(int SampleCountInRange, IReadOnlyList<SeriesPoint> Points, IReadOnlyList<TimeGap> Gaps);
}
