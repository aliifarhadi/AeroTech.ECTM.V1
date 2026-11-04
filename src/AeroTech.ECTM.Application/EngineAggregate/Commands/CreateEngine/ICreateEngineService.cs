using AeroTech.ECTM.Application.EngineAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine
{
    public interface ICreateEngineService
    {
        Task<BackofficeEngineDetailDto> CreateAsync(ICreateEngineCommand command, CancellationToken cancellationToken = default);
    }
}
