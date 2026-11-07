using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate.Contracts
{
    public interface IBaselinePrimarySelectionRepository
    {
        Task AddAsync(BaselinePrimarySelection selection, CancellationToken cancellationToken = default);

        void Remove(BaselinePrimarySelection selection);

        Task<BaselinePrimarySelection?> GetAsync(long engineId, SnapshotKind snapshotKind, string? selector, string parameterCode, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BaselinePrimarySelection>> ListForEngineAsync(long engineId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BaselinePrimarySelection>> ListForBaselineAsync(long baselineId, CancellationToken cancellationToken = default);
    }
}
