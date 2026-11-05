namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record EngineLagRules(
        double? IdleN1BelowPct,
        double? RadioHeightAboveFt,
        double? RadioHeightRiseFt,
        int? RiseLookbackSamples);
}
