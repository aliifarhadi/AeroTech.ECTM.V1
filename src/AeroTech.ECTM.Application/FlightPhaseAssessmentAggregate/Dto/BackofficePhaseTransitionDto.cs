using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePhaseTransitionDto(
        int SampleIndex,
        double Elapsed,
        EnumValueDto? FromPhase,
        EnumValueDto ToPhase,
        string? FiredRuleId,
        EnumValueDto RuleLabel,
        EnumValueDto? UnknownReason,
        string? ChangedBy,
        BackofficePredicateTraceDto? When)
    {
        public static BackofficePhaseTransitionDto From(PhaseTransition transition)
            => new(
                transition.SampleIndex,
                transition.Elapsed,
                EnumValueDto.OfNullable(transition.FromPhase),
                EnumValueDto.Of(transition.ToPhase),
                transition.FiredRuleId,
                EnumValueDto.Of(transition.RuleLabel),
                EnumValueDto.OfNullable(transition.UnknownReason),
                transition.ChangedBy,
                BackofficePredicateTraceDto.FromNullable(transition.When));
    }
}
