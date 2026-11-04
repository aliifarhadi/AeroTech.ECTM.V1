using AeroTech.ECTM.Domain.EngineModelAggregate;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.EngineModelAggregate
{
    public sealed class EngineModelRepository : IEngineModelRepository
    {
        private readonly EctmDbContext _dbContext;

        public EngineModelRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(EngineModel engineModel, CancellationToken cancellationToken = default)
            => await _dbContext.EngineModels.AddAsync(engineModel, cancellationToken);

        public Task<EngineModel?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.EngineModels.FirstOrDefaultAsync(engineModel => engineModel.Id == id, cancellationToken);

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
            => _dbContext.EngineModels.AnyAsync(engineModel => engineModel.Code == code, cancellationToken);

        public async Task<IReadOnlyList<EngineModel>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
            => await _dbContext.EngineModels
                .AsNoTracking()
                .Where(engineModel => ids.Contains(engineModel.Id))
                .ToListAsync(cancellationToken);

        public Task<PaginatedList<EngineModel>> ListAsync(string? search, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.EngineModels.AsNoTracking();

            if (search is { } text)
                query = query.Where(engineModel => engineModel.Code.Contains(text) || engineModel.Manufacturer.Contains(text));

            return query
                .OrderBy(engineModel => engineModel.Code)
                .ThenBy(engineModel => engineModel.Id)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
