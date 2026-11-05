namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
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
