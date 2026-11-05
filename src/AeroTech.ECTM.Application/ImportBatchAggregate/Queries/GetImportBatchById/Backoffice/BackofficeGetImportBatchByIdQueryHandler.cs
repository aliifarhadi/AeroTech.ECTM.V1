using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchById.Backoffice
{
    public sealed class BackofficeGetImportBatchByIdQueryHandler : IRequestHandler<BackofficeGetImportBatchByIdQuery, BackofficeImportBatchDto>
    {
        private readonly IGetImportBatchByIdService _service;

        public BackofficeGetImportBatchByIdQueryHandler(IGetImportBatchByIdService service) => _service = service;

        public Task<BackofficeImportBatchDto> Handle(BackofficeGetImportBatchByIdQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.BatchId, cancellationToken);
    }
}
