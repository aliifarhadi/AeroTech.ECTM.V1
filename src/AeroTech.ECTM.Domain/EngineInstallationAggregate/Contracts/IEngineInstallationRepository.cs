using AeroTech.ECTM.Domain.EngineInstallationAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts
{
    public interface IEngineInstallationRepository
    {
        Task AddAsync(EngineInstallation installation, CancellationToken cancellationToken = default);

        Task<EngineInstallation?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EngineInstallation>> ListForEngineOrPositionAsync(long engineId, long aircraftId, int position, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EngineInstallation>> ListByAircraftIdsAsync(IReadOnlyCollection<long> aircraftIds, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EngineInstallation>> ListOpenByAircraftIdsAsync(IReadOnlyCollection<long> aircraftIds, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EngineInstallation>> ListOpenByEngineIdsAsync(IReadOnlyCollection<long> engineIds, CancellationToken cancellationToken = default);

        Task<PaginatedList<EngineInstallation>> ListAsync(InstallationFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
