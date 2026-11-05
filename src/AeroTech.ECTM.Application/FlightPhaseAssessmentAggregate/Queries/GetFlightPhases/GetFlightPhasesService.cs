using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightPhases
{
    public sealed class GetFlightPhasesService : IGetFlightPhasesService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightPhaseAssessmentRepository _phases;
        private readonly IAnalysisRevisionRepository _revisions;

        public GetFlightPhasesService(IFlightRepository flights, IFlightPhaseAssessmentRepository phases, IAnalysisRevisionRepository revisions)
        {
            _flights = flights;
            _phases = phases;
            _revisions = revisions;
        }

        public async Task<BackofficeFlightPhasesDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var assessment = await _phases.FindCurrentAsync(flight.Id, cancellationToken)
                             ?? throw ExceptionFactory.FlightPhasesNotEvaluated(flight.Id);
            var revision = await _revisions.GetAsync(assessment.AnalysisRevisionId, cancellationToken)
                           ?? throw ExceptionFactory.FlightPhasesNotEvaluated(flight.Id);

            return BackofficeFlightPhasesDto.From(assessment, revision);
        }
    }
}
