using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePhaseRuleSetHeaderDto(
        string Code,
        string Title,
        string Description,
        EnumValueDto CalibrationStatus,
        EnumValueDto EngineeringSignOff)
    {
        public static BackofficePhaseRuleSetHeaderDto From(PhaseRuleSetHeader header)
            => new(header.Code, header.Title, header.Description, EnumValueDto.Of(header.CalibrationStatus), EnumValueDto.Of(header.EngineeringSignOff));
    }
}
