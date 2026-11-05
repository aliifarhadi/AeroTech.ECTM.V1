using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetAircraftQuality
{
    public interface IGetAircraftQualityService
    {
        Task<BackofficeAircraftQualityDto> ExecuteAsync(long aircraftId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);
    }
}
