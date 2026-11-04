using AeroTech.ECTM.Application.AuditEntryAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated.Backoffice
{
    public sealed class BackofficeGetAuditEntriesPaginatedQueryHandler : IRequestHandler<BackofficeGetAuditEntriesPaginatedQuery, PaginatedList<BackofficeAuditEntryDto>>
    {
        private readonly IGetAuditEntriesPaginatedService _service;

        public BackofficeGetAuditEntriesPaginatedQueryHandler(IGetAuditEntriesPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeAuditEntryDto>> Handle(BackofficeGetAuditEntriesPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
