using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeLabelRuleTraceDto(string RuleId, EnumValueDto Phase, bool Matched, bool Fired, BackofficePredicateTraceDto When)
    {
        public static BackofficeLabelRuleTraceDto From(LabelRuleTrace trace)
            => new(trace.RuleId, EnumValueDto.Of(trace.Label), trace.Matched, trace.Fired, BackofficePredicateTraceDto.From(trace.When));
    }
}
