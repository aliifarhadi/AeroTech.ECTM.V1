using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchesPaginated.Backoffice
{
    public sealed class BackofficeGetImportBatchesPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeImportBatchSummaryDto>>, IImportBatchesPaginatedQuery
    {
    }
}
