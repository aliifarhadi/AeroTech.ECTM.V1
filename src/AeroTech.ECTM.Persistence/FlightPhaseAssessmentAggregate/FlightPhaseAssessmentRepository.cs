using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.FlightPhaseAssessmentAggregate
{
    public sealed class FlightPhaseAssessmentRepository : IFlightPhaseAssessmentRepository
    {
        private readonly EctmDbContext _dbContext;

        public FlightPhaseAssessmentRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(FlightPhaseAssessment assessment, CancellationToken cancellationToken = default)
            => await _dbContext.FlightPhaseAssessments.AddAsync(assessment, cancellationToken);

        public Task<FlightPhaseAssessment?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => CurrentOf(flightId).FirstOrDefaultAsync(cancellationToken);

        public Task<FlightPhaseAssessment?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default)
            => CurrentOf(flightId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<FlightPhaseAssessment>> ListCurrentAsync(IReadOnlyCollection<long> flightIds, CancellationToken cancellationToken = default)
            => await _dbContext.FlightPhaseAssessments
                .AsNoTracking()
                .Where(assessment => flightIds.Contains(assessment.FlightId))
                .Where(assessment => assessment.Status != ResultStatus.SUPERSEDED)
                .ToListAsync(cancellationToken);

        private IQueryable<FlightPhaseAssessment> CurrentOf(long flightId)
            => _dbContext.FlightPhaseAssessments
                .Where(assessment => assessment.FlightId == flightId)
                .Where(assessment => assessment.Status != ResultStatus.SUPERSEDED);
    }
}
