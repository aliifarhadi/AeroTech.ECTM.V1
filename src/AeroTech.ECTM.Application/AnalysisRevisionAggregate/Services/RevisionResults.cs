using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services
{
    public sealed class RevisionResults
    {
        private readonly IFlightQualityRepository _qualities;
        private readonly IFlightPhaseAssessmentRepository _phases;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;

        public RevisionResults(IFlightQualityRepository qualities, IFlightPhaseAssessmentRepository phases, IFlightStabilityPointsRepository stabilityPoints)
        {
            _qualities = qualities;
            _phases = phases;
            _stabilityPoints = stabilityPoints;
        }

        public async Task MarkFinalAsync(long flightId, CancellationToken cancellationToken = default)
        {
            (await _qualities.GetCurrentAsync(flightId, cancellationToken))?.MarkFinal();
            (await _phases.GetCurrentAsync(flightId, cancellationToken))?.MarkFinal();
            (await _stabilityPoints.GetCurrentAsync(flightId, cancellationToken))?.MarkFinal();
        }

        public async Task MarkProvisionalAsync(long flightId, CancellationToken cancellationToken = default)
        {
            (await _qualities.GetCurrentAsync(flightId, cancellationToken))?.MarkProvisional();
            (await _phases.GetCurrentAsync(flightId, cancellationToken))?.MarkProvisional();
            (await _stabilityPoints.GetCurrentAsync(flightId, cancellationToken))?.MarkProvisional();
        }
    }
}
