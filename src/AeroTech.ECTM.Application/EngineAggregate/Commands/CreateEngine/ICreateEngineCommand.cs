using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine
{
    public interface ICreateEngineCommand
    {
        string SerialNumber { get; }

        long EngineModelId { get; }

        EngineStatus Status { get; }

        string? VariantCode { get; }

        string? ThrustRatingCode { get; }

        string? ConfigurationEvidenceReference { get; }

        LifeCountersInput? LifeCounters { get; }
    }
}
