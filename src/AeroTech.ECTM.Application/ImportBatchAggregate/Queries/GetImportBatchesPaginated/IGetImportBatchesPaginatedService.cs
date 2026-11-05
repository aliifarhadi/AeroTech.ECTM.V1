using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchesPaginated
{
    public interface IGetImportBatchesPaginatedService
    {
        Task<PaginatedList<BackofficeImportBatchSummaryDto>> ExecuteAsync(IImportBatchesPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
