using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePredicateTraceDto(
        string Operator,
        bool Holds,
        string? Channel,
        EnumValueDto? Op,
        double? Threshold,
        double? Observed,
        bool? NullAllowed,
        string? Expected,
        string? ObservedText,
        string? Reference,
        BackofficeConditionTraceDto? Condition,
        IReadOnlyList<BackofficePredicateTraceDto> Of)
    {
        public static BackofficePredicateTraceDto From(PredicateTrace trace)
            => new(
                trace.Operator,
                trace.Holds,
                trace.Channel,
                EnumValueDto.OfNullable(trace.Op),
                trace.Threshold,
                trace.Observed,
                trace.NullAllowed,
                trace.Expected,
                trace.ObservedText,
                trace.Reference,
                trace.Condition is null ? null : BackofficeConditionTraceDto.From(trace.Condition),
                (trace.Of ?? []).Select(From).ToList());

        public static BackofficePredicateTraceDto? FromNullable(PredicateTrace? trace) => trace is null ? null : From(trace);
    }
}
