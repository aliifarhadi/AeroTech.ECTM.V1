using AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft.Backoffice;
using AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft.Backoffice;
using AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById.Backoffice;
using AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftPaginated.Backoffice;
using AeroTech.ECTM.RestApi.V1.AircraftAggregate.Requests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.AircraftAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/aircraft")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BackofficeGetAircraftPaginatedQuery query, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(query, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAircraftRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeCreateAircraftCommand(request.Registration, request.TypeCode, request.OperatorCode);

            return StatusCode(StatusCodes.Status201Created, await _mediator.Send(command, cancellationToken));
        }

        [HttpGet("{aircraftId:long}")]
        public async Task<IActionResult> GetById(long aircraftId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetAircraftByIdQuery(aircraftId), cancellationToken));

        [HttpPut("{aircraftId:long}")]
        public async Task<IActionResult> Update(long aircraftId, [FromBody] UpdateAircraftRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeUpdateAircraftCommand(aircraftId, request.TypeCode, request.OperatorCode, request.Status);

            return Ok(await _mediator.Send(command, cancellationToken));
        }
    }
}
