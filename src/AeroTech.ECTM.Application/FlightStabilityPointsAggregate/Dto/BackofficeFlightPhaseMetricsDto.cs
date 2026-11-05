using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeFlightPhaseMetricsDto(
        long FlightId,
        long AnalysisRevisionId,
        long SnapshotRulesDocumentId,
        int SnapshotRulesVersion,
        EnumValueDto Status,
        IReadOnlyList<BackofficePhaseMetricGroupDto> Groups);
}
