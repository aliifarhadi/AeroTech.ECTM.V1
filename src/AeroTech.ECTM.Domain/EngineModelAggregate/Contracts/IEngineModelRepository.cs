using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Domain.EngineModelAggregate.Contracts
{
    public interface IEngineModelRepository
    {
        Task AddAsync(EngineModel engineModel, CancellationToken cancellationToken = default);

        Task<EngineModel?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EngineModel>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

        Task<PaginatedList<EngineModel>> ListAsync(string? search, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
