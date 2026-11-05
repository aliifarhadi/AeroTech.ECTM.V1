using AeroTech.ECTM.Application.FlightReviewAggregate.Queries.GetReviewReport.Backoffice;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.FlightReviewAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/reviews")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Report([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetReviewReportQuery(from, to), cancellationToken));
    }
}
