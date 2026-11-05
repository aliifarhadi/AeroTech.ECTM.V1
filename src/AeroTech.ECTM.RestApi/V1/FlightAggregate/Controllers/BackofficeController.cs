using AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightById.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightsPaginated.Backoffice;
using AeroTech.ECTM.Application.FlightQualityAggregate.Commands.EvaluateFlightQuality.Backoffice;
using AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetFlightQuality.Backoffice;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.RestApi.V1.FlightAggregate.Requests;
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
            => Ok(await _mediator.Send(new BackofficeReviewFlightCommand(flightId, request.Action, request.Note), cancellationToken));

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

        [HttpPost("{flightId:long}/quality")]
        public async Task<IActionResult> EvaluateQuality(long flightId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeEvaluateFlightQualityCommand(flightId), cancellationToken));

        [HttpGet("{flightId:long}/samples")]
        public async Task<IActionResult> Samples(long flightId, [FromQuery] int? from, [FromQuery] int? to, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetFlightSamplesQuery(flightId, from, to), cancellationToken));
    }
}
