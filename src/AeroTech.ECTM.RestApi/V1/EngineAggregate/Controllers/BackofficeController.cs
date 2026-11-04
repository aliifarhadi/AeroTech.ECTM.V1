using AeroTech.ECTM.Application.EngineAggregate.Commands;
using AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters.Backoffice;
using AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine.Backoffice;
using AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine.Backoffice;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById.Backoffice;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEnginesPaginated.Backoffice;
using AeroTech.ECTM.RestApi.V1.EngineAggregate.Requests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.EngineAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/engines")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BackofficeGetEnginesPaginatedQuery query, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(query, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEngineRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeCreateEngineCommand(
                request.SerialNumber,
                request.EngineModelId,
                request.Status,
                request.VariantCode,
                request.ThrustRatingCode,
                request.ConfigurationEvidenceReference,
                request.LifeCounters);

            return StatusCode(StatusCodes.Status201Created, await _mediator.Send(command, cancellationToken));
        }

        [HttpGet("{engineId:long}")]
        public async Task<IActionResult> GetById(long engineId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetEngineByIdQuery(engineId), cancellationToken));

        [HttpPut("{engineId:long}")]
        public async Task<IActionResult> Update(long engineId, [FromBody] UpdateEngineRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeUpdateEngineCommand(
                engineId,
                request.Status,
                request.VariantCode,
                request.ThrustRatingCode,
                request.ConfigurationEvidenceReference);

            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpPut("{engineId:long}/life-counters")]
        public async Task<IActionResult> SetLifeCounters(long engineId, [FromBody] SetLifeCountersRequest request, CancellationToken cancellationToken)
        {
            var lifeCounters = new LifeCountersInput(
                request.HoursSinceNew,
                request.CyclesSinceNew,
                request.HoursSinceLastEvent,
                request.CyclesSinceLastEvent,
                request.BasisUtc);

            var command = new BackofficeAdjustEngineLifeCountersCommand(engineId, lifeCounters, request.Reason);

            return Ok(await _mediator.Send(command, cancellationToken));
        }
    }
}
