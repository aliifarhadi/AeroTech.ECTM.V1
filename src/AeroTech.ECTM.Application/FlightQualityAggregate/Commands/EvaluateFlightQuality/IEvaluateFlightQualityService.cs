using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Commands.EvaluateFlightQuality
{
    public interface IEvaluateFlightQualityService
    {
        Task<BackofficeFlightQualityDto> EvaluateAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
