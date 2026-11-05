using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeTakeoffEgtPeakDto(
        int Position,
        EnumValueDto Availability,
        EnumValueDto? Reason,
        EnumValueDto? SearchBasis,
        int? PeakIndex,
        double? PeakElapsed,
        double? EgtPeak,
        double? OffsetFromN1PeakSec,
        IReadOnlyList<ParameterSummary> ValuesAtPeak)
    {
        public static BackofficeTakeoffEgtPeakDto From(TakeoffEgtPeakObservation peak)
            => new(
                peak.Position,
                EnumValueDto.Of(peak.Availability),
                EnumValueDto.OfNullable(peak.Reason),
                EnumValueDto.OfNullable(peak.SearchBasis),
                peak.PeakIndex,
                peak.PeakElapsed,
                peak.EgtPeak,
                peak.OffsetFromN1PeakSec,
                peak.ValuesAtPeak);
    }
}
