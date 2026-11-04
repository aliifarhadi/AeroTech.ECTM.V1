using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated.Backoffice;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds.Backoffice;
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

        [HttpGet("{kind}/documents/{documentId:long}")]
        public async Task<IActionResult> GetById(string kind, long documentId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetConfigurationDocumentByIdQuery(kind, documentId), cancellationToken));
    }
}
