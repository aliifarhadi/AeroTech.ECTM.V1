namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record RuleDecision(
        string? Id,
        string? Topic,
        string? Choice,
        string? Basis,
        string? Alternative,
        string? HowToChange,
        IReadOnlyList<string>? AppliesTo);
}
