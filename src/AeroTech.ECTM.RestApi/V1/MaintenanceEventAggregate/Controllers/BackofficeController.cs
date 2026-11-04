using AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent.Backoffice;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent.Backoffice;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Queries.GetMaintenanceEventsPaginated.Backoffice;
using AeroTech.ECTM.RestApi.V1.MaintenanceEventAggregate.Requests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.MaintenanceEventAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/maintenance-events")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BackofficeGetMaintenanceEventsPaginatedQuery query, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(query, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMaintenanceEventRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeCreateMaintenanceEventCommand(
                request.EngineId,
                request.AircraftId,
                request.Position,
                request.EventType,
                request.EventUtc,
                request.DateReliability,
                request.HoursAtEvent,
                request.CyclesAtEvent,
                request.WorkOrderReference,
                request.Notes);

            return StatusCode(StatusCodes.Status201Created, await _mediator.Send(command, cancellationToken));
        }

        [HttpPut("{maintenanceEventId:long}")]
        public async Task<IActionResult> Update(long maintenanceEventId, [FromBody] UpdateMaintenanceEventRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeUpdateMaintenanceEventCommand(
                maintenanceEventId,
                request.EventType,
                request.EventUtc,
                request.DateReliability,
                request.HoursAtEvent,
                request.CyclesAtEvent,
                request.WorkOrderReference,
                request.Notes);

            return Ok(await _mediator.Send(command, cancellationToken));
        }
    }
}
