namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record ConditionTrace(
        string Code,
        string Operator,
        bool Contains,
        PredicateTrace? Predicate = null,
        int? RunStart = null,
        int? RunEnd = null,
        double? RunSeconds = null,
        double? RequiredSec = null,
        string? Anchor = null,
        int? AnchorIndex = null,
        int? WindowStart = null,
        int? WindowEnd = null,
        PredicateTrace? Gate = null);
}
