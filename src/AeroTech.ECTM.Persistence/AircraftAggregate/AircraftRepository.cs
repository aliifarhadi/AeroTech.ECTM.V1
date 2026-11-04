using AeroTech.ECTM.Domain.AircraftAggregate;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.AircraftAggregate
{
    public sealed class AircraftRepository : IAircraftRepository
    {
        private readonly EctmDbContext _dbContext;

        public AircraftRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(Aircraft aircraft, CancellationToken cancellationToken = default)
            => await _dbContext.Aircraft.AddAsync(aircraft, cancellationToken);

        public Task<Aircraft?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.Aircraft.FirstOrDefaultAsync(aircraft => aircraft.Id == id, cancellationToken);

        public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.Aircraft.AnyAsync(aircraft => aircraft.Id == id, cancellationToken);

        public Task<bool> RegistrationExistsAsync(string registration, CancellationToken cancellationToken = default)
            => _dbContext.Aircraft.AnyAsync(aircraft => aircraft.Registration == registration, cancellationToken);

        public async Task<IReadOnlyList<Aircraft>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
            => await _dbContext.Aircraft
                .AsNoTracking()
                .Where(aircraft => ids.Contains(aircraft.Id))
                .ToListAsync(cancellationToken);

        public Task<PaginatedList<Aircraft>> ListAsync(string? search, AircraftStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Aircraft.AsNoTracking();

            if (search is { } text)
                query = query.Where(aircraft => aircraft.Registration.Contains(text));

            if (status is { } aircraftStatus)
                query = query.Where(aircraft => aircraft.Status == aircraftStatus);

            return query
                .OrderBy(aircraft => aircraft.Registration)
                .ThenBy(aircraft => aircraft.Id)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
