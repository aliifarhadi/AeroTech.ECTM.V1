using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.AircraftAggregate.Contracts
{
    public interface IAircraftRepository
    {
        Task AddAsync(Aircraft aircraft, CancellationToken cancellationToken = default);

        Task<Aircraft?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);

        Task<bool> RegistrationExistsAsync(string registration, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Aircraft>> ListByRegistrationsAsync(IReadOnlyCollection<string> registrations, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Aircraft>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

        Task<PaginatedList<Aircraft>> ListAsync(string? search, AircraftStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
