using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.RestApi.V1.EngineAggregate.Requests
{
    public sealed record UpdateEngineRequest(
        EngineStatus Status,
        string? VariantCode,
        string? ThrustRatingCode,
        string? ConfigurationEvidenceReference);
}
