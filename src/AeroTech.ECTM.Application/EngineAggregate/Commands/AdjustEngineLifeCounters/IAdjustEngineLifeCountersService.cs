using AeroTech.ECTM.Application.EngineAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters
{
    public interface IAdjustEngineLifeCountersService
    {
        Task<BackofficeEngineDetailDto> AdjustAsync(IAdjustEngineLifeCountersCommand command, CancellationToken cancellationToken = default);
    }
}
