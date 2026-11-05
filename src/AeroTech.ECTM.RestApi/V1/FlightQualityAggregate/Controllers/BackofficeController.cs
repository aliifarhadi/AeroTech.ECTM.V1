using AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetAircraftQuality.Backoffice;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.FlightQualityAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/quality")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet("aircraft/{aircraftId:long}")]
        public async Task<IActionResult> Aircraft(long aircraftId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetAircraftQualityQuery(aircraftId, from, to), cancellationToken));
    }
}
