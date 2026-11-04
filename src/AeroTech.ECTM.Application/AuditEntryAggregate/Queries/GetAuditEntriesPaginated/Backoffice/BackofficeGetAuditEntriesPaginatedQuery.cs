using AeroTech.ECTM.Application.AuditEntryAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated.Backoffice
{
    public sealed class BackofficeGetAuditEntriesPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeAuditEntryDto>>, IAuditEntriesPaginatedQuery
    {
        public string? EntityType { get; set; }

        public long? EntityId { get; set; }

        public string? Actor { get; set; }

        public DateTimeOffset? From { get; set; }

        public DateTimeOffset? To { get; set; }
    }
}
