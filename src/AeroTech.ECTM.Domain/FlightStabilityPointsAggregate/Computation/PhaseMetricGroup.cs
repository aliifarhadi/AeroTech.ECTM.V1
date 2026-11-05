using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record PhaseMetricGroup(
        MetricGroupKind Kind,
        string Name,
        FlightPhaseCode? Phase,
        int? SegmentNumber,
        string? Selector,
        int? Position,
        int StartIndex,
        int EndIndexInclusive,
        IReadOnlyList<ParameterStatistics> Parameters);
}
