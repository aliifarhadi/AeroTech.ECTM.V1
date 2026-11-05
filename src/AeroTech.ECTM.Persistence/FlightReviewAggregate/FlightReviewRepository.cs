using AeroTech.ECTM.Domain.FlightReviewAggregate;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.FlightReviewAggregate
{
    public sealed class FlightReviewRepository : IFlightReviewRepository
    {
        private readonly EctmDbContext _dbContext;

        public FlightReviewRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(FlightReview review, CancellationToken cancellationToken = default)
            => await _dbContext.FlightReviews.AddAsync(review, cancellationToken);

        public Task<FlightReview?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => CurrentOf(flightId).FirstOrDefaultAsync(cancellationToken);

        public Task<FlightReview?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => CurrentOf(flightId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<FlightReview>> ListByFlightAsync(long flightId, CancellationToken cancellationToken = default)
            => await _dbContext.FlightReviews
                .AsNoTracking()
                .Include(review => review.Entries)
                .Where(review => review.FlightId == flightId)
                .OrderByDescending(review => review.OpenedAt)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<FlightReview>> ListWithEntriesBetweenAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
            => await _dbContext.FlightReviews
                .AsNoTracking()
                .Include(review => review.Entries)
                .Where(review => review.Entries.Any(entry => entry.At >= from && entry.At < to))
                .ToListAsync(cancellationToken);

        private IQueryable<FlightReview> CurrentOf(long flightId)
            => _dbContext.FlightReviews
                .Include(review => review.Entries)
                .Where(review => review.FlightId == flightId)
                .Where(review => review.State != ReviewState.SUPERSEDED);
    }
}
