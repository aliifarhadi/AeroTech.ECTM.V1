using AeroTech.ECTM.Application.EngineAggregate.Commands;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.RestApi.V1.EngineAggregate.Requests
{
    public sealed record CreateEngineRequest(
        string SerialNumber,
        long EngineModelId,
        EngineStatus Status,
        string? VariantCode,
        string? ThrustRatingCode,
        string? ConfigurationEvidenceReference,
        LifeCountersInput? LifeCounters);
}
