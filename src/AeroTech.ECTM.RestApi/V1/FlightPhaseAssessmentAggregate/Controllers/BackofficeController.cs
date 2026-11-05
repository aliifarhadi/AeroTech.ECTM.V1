using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetAircraftEventRates.Backoffice;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.FlightPhaseAssessmentAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/events")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet("aircraft/{aircraftId:long}")]
        public async Task<IActionResult> Aircraft(long aircraftId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetAircraftEventRatesQuery(aircraftId, from, to), cancellationToken));
    }
}
