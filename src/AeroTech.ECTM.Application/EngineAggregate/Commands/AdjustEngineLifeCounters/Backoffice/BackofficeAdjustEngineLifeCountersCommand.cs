using AeroTech.ECTM.Application.EngineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters.Backoffice
{
    public sealed record BackofficeAdjustEngineLifeCountersCommand(
        long EngineId,
        LifeCountersInput LifeCounters,
        string Reason) : IRequest<BackofficeEngineDetailDto>, IAdjustEngineLifeCountersCommand;
}
