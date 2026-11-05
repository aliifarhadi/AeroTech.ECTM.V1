using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseTransition(
        int SampleIndex,
        double Elapsed,
        FlightPhaseCode? FromPhase,
        FlightPhaseCode ToPhase,
        string? FiredRuleId,
        FlightPhaseCode RuleLabel,
        PhaseUnknownReason? UnknownReason,
        string? ChangedBy,
        PredicateTrace? When);
}
