using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight.Backoffice;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Queries.GetFlightRevisions.Backoffice;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetFlightBaselineComparisons.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightById.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightsPaginated.Backoffice;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.ExplainFlightPhase.Backoffice;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightEvents.Backoffice;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightPhases.Backoffice;
using AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetFlightQuality.Backoffice;
using AeroTech.ECTM.Application.FlightReviewAggregate.Commands.BulkReviewFlights.Backoffice;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightControlContext.Backoffice;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightPhaseMetrics.Backoffice;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightStabilityPoints.Backoffice;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.RestApi.V1.BaselineAggregate.Requests;
using AeroTech.ECTM.RestApi.V1.FlightAggregate.Requests;
using AeroTech.Messages.Ectm.Enums;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.FlightAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/flights")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BackofficeGetFlightsPaginatedQuery query, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(query, cancellationToken));

        [HttpGet("{flightId:long}")]
        public async Task<IActionResult> GetById(long flightId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightByIdQuery(flightId), cancellationToken));

        [HttpPost("{flightId:long}/identity")]
        public async Task<IActionResult> ConfirmIdentity(long flightId, [FromBody] ConfirmIdentityRequest request, CancellationToken cancellationToken)
        {
            var engines = request.Engines
                .Select(engine => new EngineChoiceArgs(engine.Position, engine.EngineId, engine.PositionOnly))
                .ToList();
            var command = new BackofficeConfirmFlightIdentityCommand(
                flightId,
                request.AircraftId,
                request.FlightDate,
                request.FlightNumber,
                engines,
                request.ConfirmDistinctFlight,
                request.Note);

            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpPost("{flightId:long}/review")]
        public async Task<IActionResult> Review(long flightId, [FromBody] ReviewFlightRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeReviewFlightCommand(flightId, request.Action, request.ReasonCode, request.Note, request.AnalysisRevisionId), cancellationToken));

        [HttpGet("{flightId:long}/series")]
        public async Task<IActionResult> Series(
            long flightId,
            [FromQuery] string? parameters,
            [FromQuery] string? sampling,
            [FromQuery] int? from,
            [FromQuery] int? to,
            [FromQuery] int? budget,
            CancellationToken cancellationToken)
        {
            var query = new BackofficeGetFlightSeriesQuery(
                flightId,
                parameters ?? string.Empty,
                sampling ?? SeriesSampler.M4,
                from,
                to,
                budget ?? GetFlightSeriesService.DefaultBudget);

            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [HttpGet("{flightId:long}/quality")]
        public async Task<IActionResult> Quality(long flightId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightQualityQuery(flightId), cancellationToken));

        [HttpGet("{flightId:long}/phases")]
        public async Task<IActionResult> Phases(long flightId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightPhasesQuery(flightId), cancellationToken));

        [HttpGet("{flightId:long}/phases/explain")]
        public async Task<IActionResult> ExplainPhase(long flightId, [FromQuery] int sampleIndex, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeExplainFlightPhaseQuery(flightId, sampleIndex), cancellationToken));

        [HttpGet("{flightId:long}/stability-points")]
        public async Task<IActionResult> StabilityPoints(long flightId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightStabilityPointsQuery(flightId), cancellationToken));

        [HttpGet("{flightId:long}/phase-metrics")]
        public async Task<IActionResult> PhaseMetrics(long flightId, [FromQuery] MetricGroupKind? group, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightPhaseMetricsQuery(flightId, group), cancellationToken));

        [HttpGet("{flightId:long}/control-context")]
        public async Task<IActionResult> ControlContext(long flightId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightControlContextQuery(flightId), cancellationToken));

        [HttpPost("bulk-review")]
        public async Task<IActionResult> BulkReview([FromBody] BulkReviewFlightsRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeBulkReviewFlightsCommand(request.FlightIds, request.Action, request.ReasonCode, request.Note), cancellationToken));

        [HttpGet("{flightId:long}/events")]
        public async Task<IActionResult> Events(long flightId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightEventsQuery(flightId), cancellationToken));

        [HttpGet("{flightId:long}/revisions")]
        public async Task<IActionResult> Revisions(long flightId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightRevisionsQuery(flightId), cancellationToken));

        [HttpPost("{flightId:long}/recompute")]
        public async Task<IActionResult> Recompute(long flightId, [FromBody] RecomputeFlightRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeRecomputeFlightCommand(flightId, request.QualityRulesDocumentId, request.PhaseRuleSetDocumentId, request.SnapshotRulesDocumentId, request.DryRun ?? false);

            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpGet("{flightId:long}/samples")]
        public async Task<IActionResult> Samples(long flightId, [FromQuery] int? from, [FromQuery] int? to, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightSamplesQuery(flightId, from, to), cancellationToken));

        [HttpGet("{flightId:long}/baseline-comparisons")]
        public async Task<IActionResult> BaselineComparisons(long flightId, [FromQuery] BaselineEvaluationFilterRequest filter, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightBaselineComparisonsQuery(flightId, filter.ToArgs()), cancellationToken));
    }
}
