using AeroTech.ECTM.Domain.MaintenanceEventAggregate;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Contracts;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.MaintenanceEventAggregate
{
    public sealed class MaintenanceEventRepository : IMaintenanceEventRepository
    {
        private readonly EctmDbContext _dbContext;

        public MaintenanceEventRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(MaintenanceEvent maintenanceEvent, CancellationToken cancellationToken = default)
            => await _dbContext.MaintenanceEvents.AddAsync(maintenanceEvent, cancellationToken);

        public Task<MaintenanceEvent?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.MaintenanceEvents.FirstOrDefaultAsync(maintenanceEvent => maintenanceEvent.Id == id, cancellationToken);

        public Task<PaginatedList<MaintenanceEvent>> ListAsync(long? engineId, long? aircraftId, int? position, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.MaintenanceEvents.AsNoTracking();

            if (engineId is { } engine)
                query = query.Where(maintenanceEvent => maintenanceEvent.EngineId == engine);

            if (aircraftId is { } aircraft)
                query = query.Where(maintenanceEvent => maintenanceEvent.AircraftId == aircraft);

            if (position is { } enginePosition)
                query = query.Where(maintenanceEvent => maintenanceEvent.Position == enginePosition);

            return query
                .OrderByDescending(maintenanceEvent => maintenanceEvent.EventAt)
                .ThenByDescending(maintenanceEvent => maintenanceEvent.Id)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
