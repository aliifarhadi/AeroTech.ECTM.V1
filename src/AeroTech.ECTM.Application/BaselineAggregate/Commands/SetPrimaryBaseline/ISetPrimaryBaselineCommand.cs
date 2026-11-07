using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.SetPrimaryBaseline
{
    public interface ISetPrimaryBaselineCommand
    {
        long EngineId { get; }

        long? BaselineParameterId { get; }

        SnapshotKind? SnapshotKind { get; }

        string? Selector { get; }

        string? ParameterCode { get; }

        long? ExpectedCurrentPrimaryId { get; }
    }
}
