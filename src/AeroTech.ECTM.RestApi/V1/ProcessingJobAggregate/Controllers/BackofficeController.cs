using AeroTech.ECTM.Application.ProcessingJobAggregate.Queries.GetProcessingJobById.Backoffice;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.ProcessingJobAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/jobs")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{jobId:long}")]
        public async Task<IActionResult> GetById(long jobId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetProcessingJobByIdQuery(jobId), cancellationToken));
    }
}
