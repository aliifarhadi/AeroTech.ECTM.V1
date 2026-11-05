using AeroTech.ECTM.Domain.ImportBatchAggregate;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Computation;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Contracts;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Entities;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.ImportBatchAggregate
{
    public sealed class ImportBatchRepository : IImportBatchRepository
    {
        private readonly EctmDbContext _dbContext;

        public ImportBatchRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(ImportBatch batch, CancellationToken cancellationToken = default)
            => await _dbContext.ImportBatches.AddAsync(batch, cancellationToken);

        public Task<ImportBatch?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.ImportBatches
                .Include(batch => batch.Items)
                .FirstOrDefaultAsync(batch => batch.Id == id, cancellationToken);

        public Task<ImportBatch?> FindAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.ImportBatches
                .AsNoTracking()
                .Include(batch => batch.Items)
                .FirstOrDefaultAsync(batch => batch.Id == id, cancellationToken);

        public Task<PaginatedList<ImportBatch>> ListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
            => _dbContext.ImportBatches
                .AsNoTracking()
                .OrderByDescending(batch => batch.CreatedAt)
                .ThenByDescending(batch => batch.Id)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);

        public async Task<IReadOnlyList<ImportItemTally>> TallyItemsAsync(IReadOnlyCollection<long> batchIds, CancellationToken cancellationToken = default)
            => await _dbContext.Set<ImportItem>()
                .AsNoTracking()
                .Where(item => batchIds.Contains(item.ImportBatchId))
                .GroupBy(item => new { item.ImportBatchId, item.Outcome, item.State })
                .Select(group => new ImportItemTally(group.Key.ImportBatchId, group.Key.Outcome, group.Key.State, group.Count()))
                .ToListAsync(cancellationToken);
    }
}
