using AeroTech.ECTM.Application.BaselineAggregate.Commands.CloneBaseline.Backoffice;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.PublishBaseline.Backoffice;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.ReconcileBaseline.Backoffice;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.RetireBaseline.Backoffice;
using AeroTech.ECTM.Application.BaselineAggregate.Commands.UpdateBaseline.Backoffice;
using AeroTech.ECTM.Application.BaselineAggregate.Queries.GetBaselineById.Backoffice;
using AeroTech.ECTM.Application.BaselineAggregate.Queries.PreviewBaseline.Backoffice;
using AeroTech.ECTM.RestApi.V1.BaselineAggregate.Requests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.BaselineAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/baselines")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{baselineId:long}")]
        public async Task<IActionResult> GetById(long baselineId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetBaselineByIdQuery(baselineId), cancellationToken));

        [HttpPut("{baselineId:long}")]
        public async Task<IActionResult> Update(long baselineId, [FromBody] UpdateBaselineRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeUpdateBaselineCommand(
                baselineId,
                request.ExpectedDraftRevision,
                request.Name,
                request.DeclaredEffectiveDate,
                request.ShopVisitEventId,
                request.FirstPostShopDeclared,
                request.SourceFileName,
                request.SourceFileSha256,
                request.Notes,
                request.Parameters);

            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpPost("{baselineId:long}/preview")]
        public async Task<IActionResult> Preview(long baselineId, [FromBody] PreviewBaselineRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficePreviewBaselineQuery(baselineId, request.FlightId), cancellationToken));

        [HttpPost("{baselineId:long}/publish")]
        public async Task<IActionResult> Publish(long baselineId, [FromBody] PublishBaselineRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficePublishBaselineCommand(baselineId, request.ExpectedDraftRevision, request.ConfirmedRawReference);

            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpPost("{baselineId:long}/clone")]
        public async Task<IActionResult> Clone(long baselineId, CancellationToken cancellationToken)
            => StatusCode(StatusCodes.Status201Created, await _mediator.Send(new BackofficeCloneBaselineCommand(baselineId), cancellationToken));

        [HttpPost("{baselineId:long}/retire")]
        public async Task<IActionResult> Retire(long baselineId, [FromBody] RetireBaselineRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeRetireBaselineCommand(baselineId, request.Reason), cancellationToken));

        [HttpPost("{baselineId:long}/reconcile")]
        public async Task<IActionResult> Reconcile(long baselineId, CancellationToken cancellationToken)
            => StatusCode(StatusCodes.Status202Accepted, await _mediator.Send(new BackofficeReconcileBaselineCommand(baselineId), cancellationToken));
    }
}
