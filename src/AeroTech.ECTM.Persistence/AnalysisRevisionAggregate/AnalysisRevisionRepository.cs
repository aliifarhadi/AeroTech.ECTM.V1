using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.AnalysisRevisionAggregate
{
    public sealed class AnalysisRevisionRepository : IAnalysisRevisionRepository
    {
        private readonly EctmDbContext _dbContext;

        public AnalysisRevisionRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(AnalysisRevision revision, CancellationToken cancellationToken = default)
            => await _dbContext.AnalysisRevisions.AddAsync(revision, cancellationToken);

        public Task<AnalysisRevision?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.AnalysisRevisions.FirstOrDefaultAsync(revision => revision.Id == id, cancellationToken);

        public Task<AnalysisRevision?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => InStatus(flightId, RevisionStatus.CURRENT).FirstOrDefaultAsync(cancellationToken);

        public Task<AnalysisRevision?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => InStatus(flightId, RevisionStatus.CURRENT).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        public Task<AnalysisRevision?> FindPendingAsync(long flightId, CancellationToken cancellationToken = default)
            => InStatus(flightId, RevisionStatus.PENDING).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        public async Task<int> LastNumberAsync(long flightId, CancellationToken cancellationToken = default)
            => await _dbContext.AnalysisRevisions
                .Where(revision => revision.FlightId == flightId)
                .Select(revision => (int?)revision.Number)
                .MaxAsync(cancellationToken) ?? 0;

        public async Task<IReadOnlyList<AnalysisRevision>> ListByFlightAsync(long flightId, CancellationToken cancellationToken = default)
            => await _dbContext.AnalysisRevisions
                .AsNoTracking()
                .Where(revision => revision.FlightId == flightId)
                .OrderByDescending(revision => revision.Number)
                .ToListAsync(cancellationToken);

        private IQueryable<AnalysisRevision> InStatus(long flightId, RevisionStatus status)
            => _dbContext.AnalysisRevisions
                .Where(revision => revision.FlightId == flightId)
                .Where(revision => revision.Status == status);
    }
}
