using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeFlightStabilityPointsDto(
        long Id,
        long FlightId,
        long AnalysisRevisionId,
        long SnapshotRulesDocumentId,
        int SnapshotRulesVersion,
        BackofficeRuleDocumentHeaderDto RuleSet,
        EnumValueDto Status,
        EnumValueDto? NotEvaluableReason,
        IReadOnlyList<int> Positions,
        IReadOnlyList<BackofficeTakeoffN1PeakDto> TakeoffN1Peaks,
        IReadOnlyList<BackofficeTakeoffEgtPeakDto> TakeoffEgtPeaks,
        IReadOnlyList<BackofficeCruiseSelectorDto> Cruise,
        DateTimeOffset EvaluatedUtc)
    {
        public static BackofficeFlightStabilityPointsDto From(FlightStabilityPoints points)
        {
            var result = points.ReadResult();

            return new BackofficeFlightStabilityPointsDto(
                points.Id,
                points.FlightId,
                points.AnalysisRevisionId,
                points.SnapshotRulesDocumentId,
                points.SnapshotRulesVersion,
                BackofficeRuleDocumentHeaderDto.From(result.RuleSet),
                EnumValueDto.Of(points.Status),
                EnumValueDto.OfNullable(result.NotEvaluableReason),
                result.Positions,
                result.TakeoffN1Peaks.Select(BackofficeTakeoffN1PeakDto.From).ToList(),
                result.TakeoffEgtPeaks.Select(BackofficeTakeoffEgtPeakDto.From).ToList(),
                result.Cruise.Select(BackofficeCruiseSelectorDto.From).ToList(),
                points.EvaluatedAt);
        }
    }
}
