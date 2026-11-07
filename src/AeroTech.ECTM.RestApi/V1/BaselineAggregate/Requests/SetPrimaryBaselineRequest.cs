using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.RestApi.V1.BaselineAggregate.Requests
{
    public sealed record SetPrimaryBaselineRequest(
        long? BaselineParameterId,
        SnapshotKind? SnapshotKind,
        string? Selector,
        string? ParameterCode,
        long? ExpectedCurrentPrimaryId);
}
