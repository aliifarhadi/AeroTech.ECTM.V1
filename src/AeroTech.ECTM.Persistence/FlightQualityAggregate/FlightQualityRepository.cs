using AeroTech.ECTM.Domain.FlightQualityAggregate;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.FlightQualityAggregate
{
    public sealed class FlightQualityRepository : IFlightQualityRepository
    {
        private readonly EctmDbContext _dbContext;

        public FlightQualityRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(FlightQuality quality, CancellationToken cancellationToken = default)
            => await _dbContext.FlightQualities.AddAsync(quality, cancellationToken);

        public Task<FlightQuality?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => CurrentOf(flightId).FirstOrDefaultAsync(cancellationToken);

        public Task<FlightQuality?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => CurrentOf(flightId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<FlightQuality>> ListCurrentAsync(IReadOnlyCollection<long> flightIds, CancellationToken cancellationToken = default)
            => await _dbContext.FlightQualities
                .AsNoTracking()
                .Where(quality => flightIds.Contains(quality.FlightId))
                .Where(quality => quality.Status != ResultStatus.SUPERSEDED)
                .ToListAsync(cancellationToken);

        private IQueryable<FlightQuality> CurrentOf(long flightId)
            => _dbContext.FlightQualities
                .Where(quality => quality.FlightId == flightId)
                .Where(quality => quality.Status != ResultStatus.SUPERSEDED);
    }
}
