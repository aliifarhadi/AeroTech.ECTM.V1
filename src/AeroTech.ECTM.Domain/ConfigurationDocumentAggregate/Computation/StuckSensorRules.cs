namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record StuckSensorRules(
        double? LiveN1AtLeastPct,
        IReadOnlyList<StuckChannelRule>? Channels);
}
