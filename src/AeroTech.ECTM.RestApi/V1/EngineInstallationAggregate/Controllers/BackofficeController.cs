using AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation.Backoffice;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation.Backoffice;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Queries.GetEngineInstallationsPaginated.Backoffice;
using AeroTech.ECTM.RestApi.V1.EngineInstallationAggregate.Requests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.EngineInstallationAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/installations")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BackofficeGetEngineInstallationsPaginatedQuery query, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(query, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInstallationRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeCreateEngineInstallationCommand(
                request.EngineId,
                request.AircraftId,
                request.Position,
                request.InstalledUtc,
                request.Reason,
                request.Evidence);

            return StatusCode(StatusCodes.Status201Created, await _mediator.Send(command, cancellationToken));
        }

        [HttpPost("{installationId:long}/close")]
        public async Task<IActionResult> Close(long installationId, [FromBody] CloseInstallationRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeCloseEngineInstallationCommand(installationId, request.RemovedUtc, request.Reason);

            return Ok(await _mediator.Send(command, cancellationToken));
        }
    }
}
