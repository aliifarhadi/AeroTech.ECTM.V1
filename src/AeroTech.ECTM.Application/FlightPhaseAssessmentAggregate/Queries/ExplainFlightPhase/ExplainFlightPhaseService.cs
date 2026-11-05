using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using AeroTech.ECTM.Application.FlightQualityAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.ExplainFlightPhase
{
    public sealed class ExplainFlightPhaseService : IExplainFlightPhaseService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightPhaseAssessmentRepository _phases;
        private readonly IFlightQualityRepository _qualities;
        private readonly IAnalysisRevisionRepository _revisions;
        private readonly AnalysisRulesLoader _rules;
        private readonly FlightQualityInputLoader _inputs;

        public ExplainFlightPhaseService(
            IFlightRepository flights,
            IFlightPhaseAssessmentRepository phases,
            IFlightQualityRepository qualities,
            IAnalysisRevisionRepository revisions,
            AnalysisRulesLoader rules,
            FlightQualityInputLoader inputs)
        {
            _flights = flights;
            _phases = phases;
            _qualities = qualities;
            _revisions = revisions;
            _rules = rules;
            _inputs = inputs;
        }

        public async Task<BackofficePhaseExplanationDto> ExecuteAsync(long flightId, int sampleIndex, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);

            if (sampleIndex < 0 || sampleIndex >= flight.SampleCount)
                throw ExceptionFactory.SampleIndexOutOfRange(sampleIndex, flight.SampleCount - 1);

            var assessment = await _phases.FindCurrentAsync(flight.Id, cancellationToken)
                             ?? throw ExceptionFactory.FlightPhasesNotEvaluated(flight.Id);
            var quality = await _qualities.FindCurrentAsync(flight.Id, cancellationToken)
                          ?? throw ExceptionFactory.FlightQualityNotEvaluated(flight.Id);
            var revision = await _revisions.GetAsync(assessment.AnalysisRevisionId, cancellationToken)
                           ?? throw ExceptionFactory.FlightPhasesNotEvaluated(flight.Id);
            var rules = await _rules.LoadOfRevisionAsync(revision, cancellationToken);
            var input = await _inputs.LoadAsync(flight, rules.ParameterCodes, cancellationToken);
            var explanation = PhaseEvaluator.Explain(rules.Phase!, input, quality.ReadResult(), sampleIndex)
                              ?? throw ExceptionFactory.FlightPhasesNotEvaluated(flight.Id);

            return BackofficePhaseExplanationDto.From(flight.Id, revision.Id, assessment.PhaseRuleSetDocumentId, assessment.PhaseRuleSetVersion, explanation);
        }
    }
}
