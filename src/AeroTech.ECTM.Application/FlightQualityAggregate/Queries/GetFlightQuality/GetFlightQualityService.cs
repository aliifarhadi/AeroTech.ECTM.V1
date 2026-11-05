using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetFlightQuality
{
    public sealed class GetFlightQualityService : IGetFlightQualityService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightQualityRepository _qualities;

        public GetFlightQualityService(IFlightRepository flights, IFlightQualityRepository qualities)
        {
            _flights = flights;
            _qualities = qualities;
        }

        public async Task<BackofficeFlightQualityDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var quality = await _qualities.FindCurrentAsync(flight.Id, cancellationToken)
                          ?? throw ExceptionFactory.FlightQualityNotEvaluated(flight.Id);

            return BackofficeFlightQualityDto.From(quality);
        }
    }
}
