using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightControlContext
{
    public interface IGetFlightControlContextService
    {
        Task<BackofficeControlContextDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
