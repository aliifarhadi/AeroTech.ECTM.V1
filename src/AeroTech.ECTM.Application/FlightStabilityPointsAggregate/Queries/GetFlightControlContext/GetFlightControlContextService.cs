using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightControlContext
{
    public sealed class GetFlightControlContextService : IGetFlightControlContextService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;

        public GetFlightControlContextService(IFlightRepository flights, IFlightStabilityPointsRepository stabilityPoints)
        {
            _flights = flights;
            _stabilityPoints = stabilityPoints;
        }

        public async Task<BackofficeControlContextDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var points = await _stabilityPoints.FindCurrentAsync(flight.Id, cancellationToken)
                         ?? throw ExceptionFactory.StabilityPointsNotEvaluated(flight.Id);
            var control = points.ReadPhaseMetrics().ControlContext
                          ?? throw ExceptionFactory.StabilityPointsNotEvaluated(flight.Id);

            return new BackofficeControlContextDto(flight.Id, points.AnalysisRevisionId, control.VerificationNote, control.Phases.Select(BackofficeControlPhaseDto.From).ToList());
        }
    }
}
