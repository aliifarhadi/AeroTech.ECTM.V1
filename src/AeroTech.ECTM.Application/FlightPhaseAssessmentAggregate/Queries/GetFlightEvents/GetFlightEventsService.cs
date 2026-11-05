using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightEvents
{
    public sealed class GetFlightEventsService : IGetFlightEventsService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightPhaseAssessmentRepository _phases;

        public GetFlightEventsService(IFlightRepository flights, IFlightPhaseAssessmentRepository phases)
        {
            _flights = flights;
            _phases = phases;
        }

        public async Task<BackofficeFlightEventsDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var assessment = await _phases.FindCurrentAsync(flight.Id, cancellationToken)
                             ?? throw ExceptionFactory.FlightPhasesNotEvaluated(flight.Id);

            return BackofficeFlightEventsDto.From(assessment);
        }
    }
}
