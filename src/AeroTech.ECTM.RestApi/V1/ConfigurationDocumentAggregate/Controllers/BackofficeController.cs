using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.CreateConfigurationDraft.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.PublishConfigurationDocument.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.RetireConfigurationDocument.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.UpdateConfigurationDraft.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunOnFlight.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunPhasesOnFlight.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentDiff.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetReadiness.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetTypedDefinitionKinds.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.PreviewDataProfile;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.PreviewDataProfile.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.ValidateConfigurationDocument.Backoffice;
using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries;
using AeroTech.ECTM.RestApi.V1.ConfigurationDocumentAggregate.Requests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.ConfigurationDocumentAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/config")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet("kinds")]
        public async Task<IActionResult> Kinds(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetConfigurationKindsQuery(), cancellationToken));

        [HttpGet("{kind}/documents")]
        public async Task<IActionResult> Documents(string kind, [FromQuery] BackofficeGetConfigurationDocumentsPaginatedQuery query, CancellationToken cancellationToken)
        {
            query.Kind = kind;

            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [HttpPost("{kind}/documents")]
        public async Task<IActionResult> CreateDraft(string kind, [FromBody] CreateConfigurationDraftRequest request, CancellationToken cancellationToken)
        {
            var command = new BackofficeCreateConfigurationDraftCommand(kind, request.Content, request.BasedOnDocumentId);

            return StatusCode(StatusCodes.Status201Created, await _mediator.Send(command, cancellationToken));
        }

        [HttpGet("{kind}/documents/{documentId:long}")]
        public async Task<IActionResult> GetById(string kind, long documentId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetConfigurationDocumentByIdQuery(kind, documentId), cancellationToken));

        [HttpPut("{kind}/documents/{documentId:long}")]
        public async Task<IActionResult> UpdateDraft(string kind, long documentId, [FromBody] UpdateConfigurationDraftRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeUpdateConfigurationDraftCommand(kind, documentId, request.Content), cancellationToken));

        [HttpPost("{kind}/documents/{documentId:long}/validate")]
        public async Task<IActionResult> Validate(string kind, long documentId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeValidateConfigurationDocumentQuery(kind, documentId), cancellationToken));

        [HttpPost("{kind}/documents/{documentId:long}/publish")]
        public async Task<IActionResult> Publish(string kind, long documentId, [FromBody] PublishConfigurationDocumentRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficePublishConfigurationDocumentCommand(kind, documentId, request.ApprovalNote), cancellationToken));

        [HttpPost("{kind}/documents/{documentId:long}/retire")]
        public async Task<IActionResult> Retire(string kind, long documentId, [FromBody] RetireConfigurationDocumentRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeRetireConfigurationDocumentCommand(kind, documentId, request.Note), cancellationToken));

        [HttpPost("{kind}/documents/{documentId:long}/dry-run")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(PreviewDataProfileService.MaximumFileBytes + 1024 * 1024)]
        public async Task<IActionResult> DryRun(string kind, long documentId, IFormFile file, CancellationToken cancellationToken)
        {
            using var buffer = new MemoryStream();
            await file.CopyToAsync(buffer, cancellationToken);

            var query = new BackofficePreviewDataProfileQuery(kind, documentId, file.FileName, buffer.ToArray());

            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [HttpPost("{kind}/documents/{documentId:long}/dry-run/flight")]
        public async Task<IActionResult> DryRunOnFlight(string kind, long documentId, [FromBody] DryRunOnFlightRequest request, CancellationToken cancellationToken)
        {
            var query = new BackofficeDryRunOnFlightQuery(kind, documentId, request.FlightId, request.Budget ?? GetFlightSeriesService.DefaultBudget);

            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [HttpPost("{kind}/documents/{documentId:long}/dry-run/phases")]
        public async Task<IActionResult> DryRunPhasesOnFlight(string kind, long documentId, [FromBody] DryRunPhasesOnFlightRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeDryRunPhasesOnFlightQuery(kind, documentId, request.FlightId), cancellationToken));

        [HttpGet("{kind}/documents/{fromDocumentId:long}/diff/{toDocumentId:long}")]
        public async Task<IActionResult> Diff(string kind, long fromDocumentId, long toDocumentId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetConfigurationDocumentDiffQuery(kind, fromDocumentId, toDocumentId), cancellationToken));

        [HttpGet("typed-definitions/kinds")]
        public async Task<IActionResult> TypedDefinitionKinds(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetTypedDefinitionKindsQuery(), cancellationToken));

        [HttpPost("typed-definitions/evaluate")]
        public async Task<IActionResult> EvaluateTypedDefinition([FromBody] EvaluateTypedDefinitionRequest request, CancellationToken cancellationToken)
        {
            var query = new BackofficeEvaluateTypedDefinitionQuery(request.CatalogDocumentId, request.Definition, request.Position, request.Inputs);

            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [HttpGet("~/Backoffice/v{version:apiVersion}/meta/readiness")]
        public async Task<IActionResult> Readiness(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetReadinessQuery(), cancellationToken));
    }
}
