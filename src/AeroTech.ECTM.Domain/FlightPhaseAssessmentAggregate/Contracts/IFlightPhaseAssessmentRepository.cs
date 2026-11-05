namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts
{
    public interface IFlightPhaseAssessmentRepository
    {
        Task AddAsync(FlightPhaseAssessment assessment, CancellationToken cancellationToken = default);

        Task<FlightPhaseAssessment?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<FlightPhaseAssessment?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FlightPhaseAssessment>> ListCurrentAsync(IReadOnlyCollection<long> flightIds, CancellationToken cancellationToken = default);
    }
}
