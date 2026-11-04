namespace AeroTech.ECTM.Domain.EngineAggregate.Arguments
{
    public sealed record EngineConfigurationArgs(
        string? VariantCode,
        string? ThrustRatingCode,
        string? EvidenceReference);
}
