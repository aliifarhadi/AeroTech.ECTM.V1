using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight
{
    public interface IRecomputeFlightService
    {
        Task<BackofficeRecomputeDto> RecomputeAsync(IRecomputeFlightCommand command, CancellationToken cancellationToken = default);
    }
}
