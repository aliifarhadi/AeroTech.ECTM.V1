using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeParameterSeriesDto(
        string ParameterKey,
        string Unit,
        string Sampling,
        int SampleCountInRange,
        IReadOnlyList<SeriesPoint> Points,
        IReadOnlyList<TimeGap> Gaps);
}
