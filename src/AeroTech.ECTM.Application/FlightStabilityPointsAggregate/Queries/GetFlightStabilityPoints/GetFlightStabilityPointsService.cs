using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightStabilityPoints
{
    public sealed class GetFlightStabilityPointsService : IGetFlightStabilityPointsService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;

        public GetFlightStabilityPointsService(IFlightRepository flights, IFlightStabilityPointsRepository stabilityPoints)
        {
            _flights = flights;
            _stabilityPoints = stabilityPoints;
        }

        public async Task<BackofficeFlightStabilityPointsDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var points = await _stabilityPoints.FindCurrentAsync(flight.Id, cancellationToken)
                         ?? throw ExceptionFactory.StabilityPointsNotEvaluated(flight.Id);

            return BackofficeFlightStabilityPointsDto.From(points);
        }
    }
}
