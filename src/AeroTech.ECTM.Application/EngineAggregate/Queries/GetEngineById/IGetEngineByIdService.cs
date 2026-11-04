using AeroTech.ECTM.Application.EngineAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById
{
    public interface IGetEngineByIdService
    {
        Task<BackofficeEngineDetailDto> ExecuteAsync(long engineId, CancellationToken cancellationToken = default);
    }
}
