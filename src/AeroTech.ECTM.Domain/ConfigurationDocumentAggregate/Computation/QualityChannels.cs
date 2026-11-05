namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record QualityChannels(
        string? Altitude,
        string? RadioHeight,
        string? AirGround,
        string? Groundspeed,
        string? EngineN1,
        string? PrimaryClock,
        string? SecondaryClock);
}
