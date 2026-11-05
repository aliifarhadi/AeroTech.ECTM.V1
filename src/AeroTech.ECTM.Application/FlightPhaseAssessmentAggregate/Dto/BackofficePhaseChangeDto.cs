using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePhaseChangeDto(int StartIndex, int EndIndexInclusive, EnumValueDto? Before, EnumValueDto? After)
    {
        public static BackofficePhaseChangeDto From(PhaseChange change)
            => new(change.StartIndex, change.EndIndexInclusive, EnumValueDto.OfNullable(change.Before), EnumValueDto.OfNullable(change.After));
    }
}
