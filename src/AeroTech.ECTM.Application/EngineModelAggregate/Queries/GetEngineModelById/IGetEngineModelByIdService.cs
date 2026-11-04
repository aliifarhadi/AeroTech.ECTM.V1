using AeroTech.ECTM.Application.EngineModelAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelById
{
    public interface IGetEngineModelByIdService
    {
        Task<BackofficeEngineModelDto> ExecuteAsync(long engineModelId, CancellationToken cancellationToken = default);
    }
}
