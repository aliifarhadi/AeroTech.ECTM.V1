using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Queries.GetMaintenanceEventsPaginated.Backoffice
{
    public sealed class BackofficeGetMaintenanceEventsPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeMaintenanceEventDto>>, IMaintenanceEventsPaginatedQuery
    {
        public long? EngineId { get; set; }

        public long? AircraftId { get; set; }

        public int? Position { get; set; }
    }
}
