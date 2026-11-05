using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchesPaginated
{
    public sealed class GetImportBatchesPaginatedService : IGetImportBatchesPaginatedService
    {
        private readonly IImportBatchRepository _batches;

        public GetImportBatchesPaginatedService(IImportBatchRepository batches) => _batches = batches;

        public async Task<PaginatedList<BackofficeImportBatchSummaryDto>> ExecuteAsync(IImportBatchesPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var page = await _batches.ListAsync(query.PageNumber, query.PageSize, cancellationToken);
            var batchIds = page.Results.Select(batch => batch.Id).ToList();
            var tallies = await _batches.TallyItemsAsync(batchIds, cancellationToken);
            var talliesByBatch = tallies.ToLookup(tally => tally.ImportBatchId);

            return page.Map(batch => BackofficeImportBatchSummaryDto.From(batch, BackofficeImportCountsDto.From(talliesByBatch[batch.Id].ToList())));
        }
    }
}
