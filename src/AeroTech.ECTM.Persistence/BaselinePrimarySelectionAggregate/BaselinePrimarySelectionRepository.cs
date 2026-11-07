using AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate;
using AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.BaselinePrimarySelectionAggregate
{
    public sealed class BaselinePrimarySelectionRepository : IBaselinePrimarySelectionRepository
    {
        private readonly EctmDbContext _dbContext;

        public BaselinePrimarySelectionRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(BaselinePrimarySelection selection, CancellationToken cancellationToken = default)
            => await _dbContext.BaselinePrimarySelections.AddAsync(selection, cancellationToken);

        public void Remove(BaselinePrimarySelection selection) => _dbContext.BaselinePrimarySelections.Remove(selection);

        public Task<BaselinePrimarySelection?> GetAsync(long engineId, SnapshotKind snapshotKind, string? selector, string parameterCode, CancellationToken cancellationToken = default)
            => _dbContext.BaselinePrimarySelections
                .Where(selection => selection.EngineId == engineId)
                .Where(selection => selection.SnapshotKind == snapshotKind)
                .Where(selection => selection.Selector == selector)
                .FirstOrDefaultAsync(selection => selection.ParameterCode == parameterCode, cancellationToken);

        public async Task<IReadOnlyList<BaselinePrimarySelection>> ListForEngineAsync(long engineId, CancellationToken cancellationToken = default)
            => await _dbContext.BaselinePrimarySelections
                .AsNoTracking()
                .Where(selection => selection.EngineId == engineId)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<BaselinePrimarySelection>> ListForBaselineAsync(long baselineId, CancellationToken cancellationToken = default)
            => await _dbContext.BaselinePrimarySelections
                .Where(selection => selection.BaselineId == baselineId)
                .ToListAsync(cancellationToken);
    }
}
