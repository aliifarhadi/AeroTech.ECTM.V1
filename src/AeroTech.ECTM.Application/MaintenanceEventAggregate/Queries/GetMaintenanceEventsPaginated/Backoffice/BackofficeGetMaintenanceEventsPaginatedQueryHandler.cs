using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Queries.GetMaintenanceEventsPaginated.Backoffice
{
    public sealed class BackofficeGetMaintenanceEventsPaginatedQueryHandler : IRequestHandler<BackofficeGetMaintenanceEventsPaginatedQuery, PaginatedList<BackofficeMaintenanceEventDto>>
    {
        private readonly IGetMaintenanceEventsPaginatedService _service;

        public BackofficeGetMaintenanceEventsPaginatedQueryHandler(IGetMaintenanceEventsPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeMaintenanceEventDto>> Handle(BackofficeGetMaintenanceEventsPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
