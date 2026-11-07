using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.SetPrimaryBaseline.Backoffice
{
    public sealed record BackofficeSetPrimaryBaselineCommand(
        long EngineId,
        long? BaselineParameterId,
        SnapshotKind? SnapshotKind,
        string? Selector,
        string? ParameterCode,
        long? ExpectedCurrentPrimaryId) : IRequest<BackofficeEngineBaselinesDto>, ISetPrimaryBaselineCommand;
}
