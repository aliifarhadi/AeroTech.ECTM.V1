using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CloneBaseline.Backoffice
{
    public sealed record BackofficeCloneBaselineCommand(
        long BaselineId) : IRequest<BackofficeBaselineDto>, ICloneBaselineCommand;
}
