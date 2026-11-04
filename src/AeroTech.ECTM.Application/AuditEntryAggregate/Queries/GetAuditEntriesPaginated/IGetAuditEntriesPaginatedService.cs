using AeroTech.ECTM.Application.AuditEntryAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated
{
    public interface IGetAuditEntriesPaginatedService
    {
        Task<PaginatedList<BackofficeAuditEntryDto>> ExecuteAsync(IAuditEntriesPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
