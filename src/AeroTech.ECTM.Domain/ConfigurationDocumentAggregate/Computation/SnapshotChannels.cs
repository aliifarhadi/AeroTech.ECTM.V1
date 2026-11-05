namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record SnapshotChannels(
        string? EngineN1,
        string? EngineEgt,
        string? Altitude,
        string? Mach,
        string? Toga);
}
