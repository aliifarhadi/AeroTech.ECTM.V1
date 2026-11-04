using AeroTech.ECTM.Domain.EngineAggregate;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.EngineAggregate
{
    public sealed class EngineRepository : IEngineRepository
    {
        private readonly EctmDbContext _dbContext;

        public EngineRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(Engine engine, CancellationToken cancellationToken = default)
            => await _dbContext.Engines.AddAsync(engine, cancellationToken);

        public Task<Engine?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.Engines
                .Include(engine => engine.LifeCounterAdjustments)
                .FirstOrDefaultAsync(engine => engine.Id == id, cancellationToken);

        public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.Engines.AnyAsync(engine => engine.Id == id, cancellationToken);

        public Task<bool> SerialExistsAsync(string manufacturer, string serialNumber, CancellationToken cancellationToken = default)
            => _dbContext.Engines.AnyAsync(engine => engine.Manufacturer == manufacturer && engine.SerialNumber == serialNumber, cancellationToken);

        public async Task<IReadOnlyList<Engine>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
            => await _dbContext.Engines
                .AsNoTracking()
                .Where(engine => ids.Contains(engine.Id))
                .ToListAsync(cancellationToken);

        public Task<PaginatedList<Engine>> ListAsync(string? search, long? engineModelId, EngineStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Engines.AsNoTracking();

            if (search is { } text)
                query = query.Where(engine => engine.SerialNumber.Contains(text));

            if (engineModelId is { } modelId)
                query = query.Where(engine => engine.EngineModelId == modelId);

            if (status is { } engineStatus)
                query = query.Where(engine => engine.Status == engineStatus);

            return query
                .OrderBy(engine => engine.SerialNumber)
                .ThenBy(engine => engine.Id)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
