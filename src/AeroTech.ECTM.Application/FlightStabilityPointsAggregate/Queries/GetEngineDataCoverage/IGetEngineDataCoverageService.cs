using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEngineDataCoverage
{
    public interface IGetEngineDataCoverageService
    {
        Task<BackofficeEngineDataCoverageDto> ExecuteAsync(long engineId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);
    }
}
