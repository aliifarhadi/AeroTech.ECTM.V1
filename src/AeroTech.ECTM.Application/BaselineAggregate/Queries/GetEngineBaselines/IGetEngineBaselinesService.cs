using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.GetEngineBaselines
{
    public interface IGetEngineBaselinesService
    {
        Task<BackofficeEngineBaselinesDto> ExecuteAsync(long engineId, CancellationToken cancellationToken = default);
    }
}
