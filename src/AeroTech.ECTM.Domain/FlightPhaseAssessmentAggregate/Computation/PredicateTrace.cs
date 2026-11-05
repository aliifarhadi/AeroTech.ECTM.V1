using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PredicateTrace(
        string Operator,
        bool Holds,
        string? Channel = null,
        CompareOperator? Op = null,
        double? Threshold = null,
        double? Observed = null,
        bool? NullAllowed = null,
        string? Expected = null,
        string? ObservedText = null,
        string? Reference = null,
        ConditionTrace? Condition = null,
        IReadOnlyList<PredicateTrace>? Of = null);
}
