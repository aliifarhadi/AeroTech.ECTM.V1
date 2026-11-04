using AeroTech.ECTM.Application.AuditEntryAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.AuditEntryAggregate.Arguments;
using AeroTech.ECTM.Domain.AuditEntryAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated
{
    public sealed class GetAuditEntriesPaginatedService : IGetAuditEntriesPaginatedService
    {
        private readonly IAuditEntryRepository _auditEntries;

        public GetAuditEntriesPaginatedService(IAuditEntryRepository auditEntries) => _auditEntries = auditEntries;

        public async Task<PaginatedList<BackofficeAuditEntryDto>> ExecuteAsync(IAuditEntriesPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var filter = new AuditEntryFilterArgs(query.EntityType, query.EntityId, query.Actor, query.From, query.To);
            var page = await _auditEntries.ListAsync(filter, query.PageNumber, query.PageSize, cancellationToken);

            return page.Map(BackofficeAuditEntryDto.From);
        }
    }
}
