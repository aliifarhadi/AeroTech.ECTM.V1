using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeTakeoffN1PeakDto(
        int Position,
        EnumValueDto Availability,
        EnumValueDto? Reason,
        EnumValueDto? SearchBasis,
        int? SearchStartIndex,
        int? SearchEndIndexInclusive,
        int? PeakIndex,
        double? PeakElapsed,
        int? StartIndex,
        int? EndIndexInclusive,
        int? ValidCount,
        int? ValidEgtCount,
        double? N1Peak,
        double? EgtAtPeak,
        IReadOnlyList<ParameterSummary> Values,
        SnapshotContext? Context,
        EnumValueDto? ContextStatus)
    {
        public static BackofficeTakeoffN1PeakDto From(TakeoffN1PeakContext peak)
            => new(
                peak.Position,
                EnumValueDto.Of(peak.Availability),
                EnumValueDto.OfNullable(peak.Reason),
                EnumValueDto.OfNullable(peak.SearchBasis),
                peak.SearchStartIndex,
                peak.SearchEndIndexInclusive,
                peak.PeakIndex,
                peak.PeakElapsed,
                peak.StartIndex,
                peak.EndIndexInclusive,
                peak.ValidCount,
                peak.ValidEgtCount,
                peak.N1Peak,
                peak.EgtAtPeak,
                peak.Values,
                peak.Context,
                EnumValueDto.OfNullable(peak.ContextStatus));
    }
}
