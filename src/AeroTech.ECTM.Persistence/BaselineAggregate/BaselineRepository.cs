using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.BaselineAggregate
{
    public sealed class BaselineRepository : IBaselineRepository
    {
        private readonly EctmDbContext _dbContext;

        public BaselineRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(Baseline baseline, CancellationToken cancellationToken = default)
            => await _dbContext.Baselines.AddAsync(baseline, cancellationToken);

        public Task<Baseline?> GetAsync(long baselineId, CancellationToken cancellationToken = default)
            => WithParameters().FirstOrDefaultAsync(baseline => baseline.Id == baselineId, cancellationToken);

        public Task<Baseline?> FindAsync(long baselineId, CancellationToken cancellationToken = default)
            => WithParameters().AsNoTracking().FirstOrDefaultAsync(baseline => baseline.Id == baselineId, cancellationToken);

        public Task<Baseline?> GetByParameterIdAsync(long baselineParameterId, CancellationToken cancellationToken = default)
            => WithParameters().FirstOrDefaultAsync(baseline => baseline.Parameters.Any(parameter => parameter.Id == baselineParameterId), cancellationToken);

        public async Task<IReadOnlyList<Baseline>> ListForEngineAsync(long engineId, CancellationToken cancellationToken = default)
            => await WithParameters()
                .AsNoTracking()
                .Where(baseline => baseline.EngineId == engineId)
                .OrderByDescending(baseline => baseline.CreatedAt)
                .ThenByDescending(baseline => baseline.Id)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Baseline>> ListEvaluatedForEngineAsync(long engineId, CancellationToken cancellationToken = default)
            => await WithParameters()
                .AsNoTracking()
                .Where(baseline => baseline.EngineId == engineId)
                .Where(baseline => baseline.State != BaselineState.DRAFT)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Baseline>> ListByIdsAsync(IReadOnlyCollection<long> baselineIds, CancellationToken cancellationToken = default)
            => await WithParameters()
                .AsNoTracking()
                .Where(baseline => baselineIds.Contains(baseline.Id))
                .ToListAsync(cancellationToken);

        public Task<bool> AnyEvaluatedForEngineAsync(long engineId, CancellationToken cancellationToken = default)
            => _dbContext.Baselines
                .Where(baseline => baseline.EngineId == engineId)
                .AnyAsync(baseline => baseline.State != BaselineState.DRAFT, cancellationToken);

        public async Task<int> LastRevisionNumberAsync(long lineageId, CancellationToken cancellationToken = default)
            => await _dbContext.Baselines
                .Where(baseline => baseline.LineageId == lineageId)
                .MaxAsync(baseline => baseline.RevisionNumber, cancellationToken);

        private IQueryable<Baseline> WithParameters()
            => _dbContext.Baselines.Include(baseline => baseline.Parameters);
    }
}
