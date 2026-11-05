using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeDerivedDryRunDto(
        string ParameterKey,
        string Code,
        int? Position,
        string Kind,
        string? UnitOut,
        string? DefinitionStatus,
        string AgainstActive,
        int AvailableCount,
        IReadOnlyList<BackofficeStatusCountDto> StatusCounts,
        double? Minimum,
        double? Maximum,
        int? FirstAvailableSample,
        string? FirstAvailableFormula,
        IReadOnlyList<SeriesPoint> Points,
        IReadOnlyList<TimeGap> Gaps)
    {
        public static BackofficeDerivedDryRunDto From(DerivedSeries series, string againstActive, SampledSeries? sampled)
            => new(
                series.ParameterKey,
                series.Code,
                series.Position,
                series.Kind,
                series.UnitOut,
                series.DefinitionStatus,
                againstActive,
                series.AvailableCount,
                series.StatusCounts
                    .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                    .Select(pair => new BackofficeStatusCountDto(pair.Key, pair.Value))
                    .ToList(),
                series.Minimum,
                series.Maximum,
                series.FirstAvailableSample,
                series.FirstAvailableFormula,
                sampled?.Points ?? [],
                sampled?.Gaps ?? []);
    }
}
