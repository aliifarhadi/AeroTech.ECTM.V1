using AeroTech.ECTM.Application.EngineAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine
{
    public interface IUpdateEngineService
    {
        Task<BackofficeEngineDetailDto> UpdateAsync(IUpdateEngineCommand command, CancellationToken cancellationToken = default);
    }
}
