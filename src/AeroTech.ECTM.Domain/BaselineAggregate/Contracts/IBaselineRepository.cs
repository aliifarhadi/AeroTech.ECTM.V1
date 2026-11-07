namespace AeroTech.ECTM.Domain.BaselineAggregate.Contracts
{
    public interface IBaselineRepository
    {
        Task AddAsync(Baseline baseline, CancellationToken cancellationToken = default);

        Task<Baseline?> GetAsync(long baselineId, CancellationToken cancellationToken = default);

        Task<Baseline?> FindAsync(long baselineId, CancellationToken cancellationToken = default);

        Task<Baseline?> GetByParameterIdAsync(long baselineParameterId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Baseline>> ListForEngineAsync(long engineId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Baseline>> ListEvaluatedForEngineAsync(long engineId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Baseline>> ListByIdsAsync(IReadOnlyCollection<long> baselineIds, CancellationToken cancellationToken = default);

        Task<bool> AnyEvaluatedForEngineAsync(long engineId, CancellationToken cancellationToken = default);

        Task<int> LastRevisionNumberAsync(long lineageId, CancellationToken cancellationToken = default);
    }
}
