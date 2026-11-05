using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeEngine
{
    public interface IRecomputeEngineService
    {
        Task<BackofficeEngineRecomputeDto> RecomputeAsync(IRecomputeEngineCommand command, CancellationToken cancellationToken = default);
    }
}
