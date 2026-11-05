using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeConditionTraceDto(
        string Code,
        string Operator,
        bool Contains,
        BackofficePredicateTraceDto? Predicate,
        int? RunStart,
        int? RunEnd,
        double? RunSeconds,
        double? RequiredSec,
        string? Anchor,
        int? AnchorIndex,
        int? WindowStart,
        int? WindowEnd,
        BackofficePredicateTraceDto? Gate)
    {
        public static BackofficeConditionTraceDto From(ConditionTrace trace)
            => new(
                trace.Code,
                trace.Operator,
                trace.Contains,
                BackofficePredicateTraceDto.FromNullable(trace.Predicate),
                trace.RunStart,
                trace.RunEnd,
                trace.RunSeconds,
                trace.RequiredSec,
                trace.Anchor,
                trace.AnchorIndex,
                trace.WindowStart,
                trace.WindowEnd,
                BackofficePredicateTraceDto.FromNullable(trace.Gate));
    }
}
