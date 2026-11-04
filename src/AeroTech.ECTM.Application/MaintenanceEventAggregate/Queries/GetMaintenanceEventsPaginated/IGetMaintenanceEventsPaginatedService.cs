using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Queries.GetMaintenanceEventsPaginated
{
    public interface IGetMaintenanceEventsPaginatedService
    {
        Task<PaginatedList<BackofficeMaintenanceEventDto>> ExecuteAsync(IMaintenanceEventsPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
