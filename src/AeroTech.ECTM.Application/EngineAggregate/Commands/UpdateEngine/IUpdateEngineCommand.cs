using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine
{
    public interface IUpdateEngineCommand
    {
        long EngineId { get; }

        EngineStatus Status { get; }

        string? VariantCode { get; }

        string? ThrustRatingCode { get; }

        string? ConfigurationEvidenceReference { get; }
    }
}
