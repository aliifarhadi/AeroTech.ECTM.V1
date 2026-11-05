using AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel.Backoffice;
using AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel.Backoffice;
using AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelById.Backoffice;
using AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelsPaginated.Backoffice;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetModelPeriodPerformance.Backoffice;
using AeroTech.ECTM.RestApi.V1.EngineModelAggregate.Requests;
using AeroTech.Messages.Ectm.Enums;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.EngineModelAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/engine-models")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BackofficeGetEngineModelsPaginatedQuery query, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(query, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEngineModelRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeCreateEngineModelCommand(
                request.Code,
                request.Manufacturer,
                request.Family,
                request.VariantCode,
                request.ThrustRatingCode,
                request.DocumentReferences ?? [],
                request.Notes);

            return StatusCode(StatusCodes.Status201Created, await _mediator.Send(command, cancellationToken));
        }

        [HttpGet("{engineModelId:long}")]
        public async Task<IActionResult> GetById(long engineModelId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetEngineModelByIdQuery(engineModelId), cancellationToken));

        [HttpGet("{engineModelId:long}/period-performance")]
        public async Task<IActionResult> PeriodPerformance(
            long engineModelId,
            [FromQuery] SnapshotKind kind,
            [FromQuery] string? selector,
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to,
            CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetModelPeriodPerformanceQuery(engineModelId, from, to, kind, selector), cancellationToken));

        [HttpPut("{engineModelId:long}")]
        public async Task<IActionResult> Update(long engineModelId, [FromBody] UpdateEngineModelRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeUpdateEngineModelCommand(
                engineModelId,
                request.Manufacturer,
                request.Family,
                request.VariantCode,
                request.ThrustRatingCode,
                request.DocumentReferences ?? [],
                request.Notes);

            return Ok(await _mediator.Send(command, cancellationToken));
        }
    }
}
