using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Domain.MaintenanceEventAggregate.Contracts
{
    public interface IMaintenanceEventRepository
    {
        Task AddAsync(MaintenanceEvent maintenanceEvent, CancellationToken cancellationToken = default);

        Task<MaintenanceEvent?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<PaginatedList<MaintenanceEvent>> ListAsync(long? engineId, long? aircraftId, int? position, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
