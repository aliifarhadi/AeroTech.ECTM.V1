using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunPhasesOnFlight
{
    public sealed class DryRunPhasesOnFlightService : IDryRunPhasesOnFlightService
    {
        private readonly ConfigurationDocumentLoader _documentLoader;
        private readonly IFlightRepository _flights;
        private readonly IAnalysisRevisionRepository _revisions;
        private readonly AnalysisRulesLoader _rules;
        private readonly PhasePreviewer _previewer;

        public DryRunPhasesOnFlightService(
            ConfigurationDocumentLoader documentLoader,
            IFlightRepository flights,
            IAnalysisRevisionRepository revisions,
            AnalysisRulesLoader rules,
            PhasePreviewer previewer)
        {
            _documentLoader = documentLoader;
            _flights = flights;
            _revisions = revisions;
            _rules = rules;
            _previewer = previewer;
        }

        public async Task<BackofficePhaseDryRunDto> ExecuteAsync(string kind, long documentId, long flightId, CancellationToken cancellationToken = default)
        {
            var document = await _documentLoader.LoadAsync(kind, documentId, cancellationToken);

            if (document.Kind != ConfigurationKind.PHASE_RULESET)
                throw ExceptionFactory.DryRunNotAvailable(document.Kind, ConfigurationKinds.DeliveryPhase[document.Kind]);

            var flight = await _flights.GetAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var revision = await _revisions.FindCurrentAsync(flight.Id, cancellationToken);
            var rules = await _rules.LoadWithPhaseDraftAsync(revision, document, cancellationToken);

            return await _previewer.PreviewAsync(flight, rules, cancellationToken);
        }
    }
}
