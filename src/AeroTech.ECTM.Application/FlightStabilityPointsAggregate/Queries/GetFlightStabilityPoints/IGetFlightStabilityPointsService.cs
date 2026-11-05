using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightStabilityPoints
{
    public interface IGetFlightStabilityPointsService
    {
        Task<BackofficeFlightStabilityPointsDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
