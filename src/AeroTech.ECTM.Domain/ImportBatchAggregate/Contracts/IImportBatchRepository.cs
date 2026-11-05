using AeroTech.ECTM.Domain.ImportBatchAggregate.Computation;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Domain.ImportBatchAggregate.Contracts
{
    public interface IImportBatchRepository
    {
        Task AddAsync(ImportBatch batch, CancellationToken cancellationToken = default);

        Task<ImportBatch?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<ImportBatch?> FindAsync(long id, CancellationToken cancellationToken = default);

        Task<PaginatedList<ImportBatch>> ListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ImportItemTally>> TallyItemsAsync(IReadOnlyCollection<long> batchIds, CancellationToken cancellationToken = default);
    }
}
