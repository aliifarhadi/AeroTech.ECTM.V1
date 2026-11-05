using AeroTech.ECTM.Application.ImportBatchAggregate.Commands.CommitImport.Backoffice;
using AeroTech.ECTM.Application.ImportBatchAggregate.Commands.PreviewImport.Backoffice;
using AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchById.Backoffice;
using AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchesPaginated.Backoffice;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using AeroTech.ECTM.RestApi.V1.ImportBatchAggregate.Requests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroTech.ECTM.RestApi.V1.ImportBatchAggregate
{
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Backoffice")]
    [Route($"Backoffice/v{{version:apiVersion}}/imports")]
    public sealed class BackofficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BackofficeController(IMediator mediator) => _mediator = mediator;

        [HttpPost("preview")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(ImportOptions.RequestBodyLimitBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = ImportOptions.RequestBodyLimitBytes)]
        public async Task<IActionResult> Preview([FromForm] PreviewImportRequest request, CancellationToken cancellationToken)
        {
            var files = new List<UploadedFile>();

            foreach (var file in request.Files)
            {
                using var buffer = new MemoryStream();
                await file.CopyToAsync(buffer, cancellationToken);

                files.Add(new UploadedFile(file.FileName, buffer.ToArray()));
            }

            var command = new BackofficePreviewImportCommand(files, request.DefaultRegistration, request.DefaultYear);

            return StatusCode(StatusCodes.Status201Created, await _mediator.Send(command, cancellationToken));
        }

        [HttpPost("commit")]
        public async Task<IActionResult> Commit([FromBody] CommitImportRequest request, CancellationToken cancellationToken)
            => StatusCode(StatusCodes.Status202Accepted, await _mediator.Send(new BackofficeCommitImportCommand(request.BatchId, request.IdempotencyKey), cancellationToken));

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BackofficeGetImportBatchesPaginatedQuery query, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(query, cancellationToken));

        [HttpGet("{batchId:long}")]
        public async Task<IActionResult> GetById(long batchId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new BackofficeGetImportBatchByIdQuery(batchId), cancellationToken));
    }
}
