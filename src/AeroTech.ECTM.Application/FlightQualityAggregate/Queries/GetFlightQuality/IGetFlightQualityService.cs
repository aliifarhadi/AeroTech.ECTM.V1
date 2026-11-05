using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetFlightQuality
{
    public interface IGetFlightQualityService
    {
        Task<BackofficeFlightQualityDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
