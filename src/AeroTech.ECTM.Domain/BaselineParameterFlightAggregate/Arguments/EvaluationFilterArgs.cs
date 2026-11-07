using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments
{
    public sealed record EvaluationFilterArgs(
        IReadOnlyCollection<long>? BaselineIds,
        SnapshotKind? SnapshotKind,
        string? Selector,
        string? ParameterCode,
        DateOnly? From,
        DateOnly? To);
}
