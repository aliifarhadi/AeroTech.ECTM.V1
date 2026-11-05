namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record CoverageRules(
        IReadOnlyList<string>? RequiredChannels,
        double? WarningBelowPct,
        double? BlockingBelowPct);
}
