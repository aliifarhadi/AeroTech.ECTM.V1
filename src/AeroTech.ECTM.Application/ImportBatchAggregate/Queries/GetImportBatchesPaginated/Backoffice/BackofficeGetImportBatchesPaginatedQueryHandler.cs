using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchesPaginated.Backoffice
{
    public sealed class BackofficeGetImportBatchesPaginatedQueryHandler : IRequestHandler<BackofficeGetImportBatchesPaginatedQuery, PaginatedList<BackofficeImportBatchSummaryDto>>
    {
        private readonly IGetImportBatchesPaginatedService _service;

        public BackofficeGetImportBatchesPaginatedQueryHandler(IGetImportBatchesPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeImportBatchSummaryDto>> Handle(BackofficeGetImportBatchesPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
