using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record AnchorResult(
        string Code,
        string Operator,
        int? Index,
        double? Elapsed,
        AnchorBasis Basis,
        string Channel,
        double? NumberBefore = null,
        double? NumberAt = null,
        string? TextBefore = null,
        string? TextAt = null,
        PredicateTrace? At = null);
}
