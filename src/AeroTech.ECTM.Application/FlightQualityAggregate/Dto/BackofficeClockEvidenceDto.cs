using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Dto
{
    public sealed record BackofficeClockEvidenceDto(
        int DisagreeRows,
        int InvalidOrMissingRows,
        int PrimaryNotInStepRows,
        int SecondaryNotInStepRows,
        IReadOnlyDictionary<string, int> OffsetHistogram,
        EnumValueDto TimeTrust)
    {
        public static BackofficeClockEvidenceDto From(ClockEvidence clock)
            => new(
                clock.DisagreeRows,
                clock.InvalidOrMissingRows,
                clock.PrimaryNotInStepRows,
                clock.SecondaryNotInStepRows,
                clock.OffsetHistogram,
                EnumValueDto.Of(clock.TimeTrust));
    }
}
