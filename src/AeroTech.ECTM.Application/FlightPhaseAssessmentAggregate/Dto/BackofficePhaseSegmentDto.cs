using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePhaseSegmentDto(
        EnumValueDto Phase,
        int StartIndex,
        int EndIndexInclusive,
        double StartElapsed,
        double EndElapsed,
        double Seconds,
        EnumValueDto Confidence,
        IReadOnlyList<string> EvidenceCodes,
        IReadOnlyList<string> Flags,
        IReadOnlyList<EnumValueDto> UnknownReasons)
    {
        public static BackofficePhaseSegmentDto From(PhaseSegment segment)
            => new(
                EnumValueDto.Of(segment.PhaseCode),
                segment.StartIndex,
                segment.EndIndexInclusive,
                segment.StartElapsed,
                segment.EndElapsed,
                segment.Seconds,
                EnumValueDto.Of(segment.Confidence),
                segment.EvidenceCodes,
                segment.Flags,
                segment.UnknownReasons.Select(EnumValueDto.Of).ToList());
    }
}
