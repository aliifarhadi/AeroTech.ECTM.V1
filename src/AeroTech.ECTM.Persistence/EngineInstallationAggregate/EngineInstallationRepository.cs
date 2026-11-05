using AeroTech.ECTM.Domain.EngineInstallationAggregate;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Arguments;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.EngineInstallationAggregate
{
    public sealed class EngineInstallationRepository : IEngineInstallationRepository
    {
        private readonly EctmDbContext _dbContext;

        public EngineInstallationRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(EngineInstallation installation, CancellationToken cancellationToken = default)
            => await _dbContext.EngineInstallations.AddAsync(installation, cancellationToken);

        public Task<EngineInstallation?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.EngineInstallations
                .Include(installation => installation.Changes)
                .FirstOrDefaultAsync(installation => installation.Id == id, cancellationToken);

        public async Task<IReadOnlyList<EngineInstallation>> ListForEngineOrPositionAsync(long engineId, long aircraftId, int position, CancellationToken cancellationToken = default)
            => await _dbContext.EngineInstallations
                .AsNoTracking()
                .Where(installation => installation.EngineId == engineId
                                       || (installation.AircraftId == aircraftId && installation.Position == position))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<EngineInstallation>> ListByAircraftIdsAsync(IReadOnlyCollection<long> aircraftIds, CancellationToken cancellationToken = default)
            => await _dbContext.EngineInstallations
                .AsNoTracking()
                .Where(installation => aircraftIds.Contains(installation.AircraftId))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<EngineInstallation>> ListOpenByAircraftIdsAsync(IReadOnlyCollection<long> aircraftIds, CancellationToken cancellationToken = default)
            => await _dbContext.EngineInstallations
                .AsNoTracking()
                .Include(installation => installation.Changes)
                .Where(installation => aircraftIds.Contains(installation.AircraftId))
                .Where(installation => installation.RemovedAt == null)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<EngineInstallation>> ListOpenByEngineIdsAsync(IReadOnlyCollection<long> engineIds, CancellationToken cancellationToken = default)
            => await _dbContext.EngineInstallations
                .AsNoTracking()
                .Include(installation => installation.Changes)
                .Where(installation => engineIds.Contains(installation.EngineId))
                .Where(installation => installation.RemovedAt == null)
                .ToListAsync(cancellationToken);

        public Task<PaginatedList<EngineInstallation>> ListAsync(InstallationFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.EngineInstallations
                .AsNoTracking()
                .Include(installation => installation.Changes)
                .AsQueryable();

            if (filter.EngineId is { } engineId)
                query = query.Where(installation => installation.EngineId == engineId);

            if (filter.AircraftId is { } aircraftId)
                query = query.Where(installation => installation.AircraftId == aircraftId);

            if (filter.Position is { } position)
                query = query.Where(installation => installation.Position == position);

            if (filter.OpenOnly)
                query = query.Where(installation => installation.RemovedAt == null);

            return query
                .OrderByDescending(installation => installation.InstalledAt)
                .ThenByDescending(installation => installation.Id)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
