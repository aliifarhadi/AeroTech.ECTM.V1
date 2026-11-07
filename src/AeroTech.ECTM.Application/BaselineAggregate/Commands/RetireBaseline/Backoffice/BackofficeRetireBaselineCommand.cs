using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.RetireBaseline.Backoffice
{
    public sealed record BackofficeRetireBaselineCommand(
        long BaselineId,
        string Reason) : IRequest<BackofficeBaselineDto>, IRetireBaselineCommand;
}
