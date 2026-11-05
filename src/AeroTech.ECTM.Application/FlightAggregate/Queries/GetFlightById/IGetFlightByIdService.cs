using AeroTech.ECTM.Application.FlightAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightById
{
    public interface IGetFlightByIdService
    {
        Task<BackofficeFlightDetailDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
