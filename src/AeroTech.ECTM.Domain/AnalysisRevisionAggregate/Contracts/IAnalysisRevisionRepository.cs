namespace AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts
{
    public interface IAnalysisRevisionRepository
    {
        Task AddAsync(AnalysisRevision revision, CancellationToken cancellationToken = default);

        Task<AnalysisRevision?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<AnalysisRevision?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<AnalysisRevision?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<AnalysisRevision?> FindPendingAsync(long flightId, CancellationToken cancellationToken = default);

        Task<int> LastNumberAsync(long flightId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AnalysisRevision>> ListByFlightAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
