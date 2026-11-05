using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.FlightStabilityPointsAggregate
{
    public sealed class FlightStabilityPointsRepository : IFlightStabilityPointsRepository
    {
        private readonly EctmDbContext _dbContext;

        public FlightStabilityPointsRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(FlightStabilityPoints points, CancellationToken cancellationToken = default)
            => await _dbContext.FlightStabilityPoints.AddAsync(points, cancellationToken);

        public Task<FlightStabilityPoints?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => CurrentOf(flightId).FirstOrDefaultAsync(cancellationToken);

        public Task<FlightStabilityPoints?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => CurrentOf(flightId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<FlightStabilityPoints>> ListCurrentAsync(IReadOnlyCollection<long> flightIds, CancellationToken cancellationToken = default)
            => await _dbContext.FlightStabilityPoints
                .AsNoTracking()
                .Where(points => flightIds.Contains(points.FlightId))
                .Where(points => points.Status != ResultStatus.SUPERSEDED)
                .ToListAsync(cancellationToken);

        private IQueryable<FlightStabilityPoints> CurrentOf(long flightId)
            => _dbContext.FlightStabilityPoints
                .Where(points => points.FlightId == flightId)
                .Where(points => points.Status != ResultStatus.SUPERSEDED);
    }
}
