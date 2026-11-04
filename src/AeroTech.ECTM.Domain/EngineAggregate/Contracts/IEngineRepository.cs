using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.EngineAggregate.Contracts
{
    public interface IEngineRepository
    {
        Task AddAsync(Engine engine, CancellationToken cancellationToken = default);

        Task<Engine?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);

        Task<bool> SerialExistsAsync(string manufacturer, string serialNumber, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Engine>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

        Task<PaginatedList<Engine>> ListAsync(string? search, long? engineModelId, EngineStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
