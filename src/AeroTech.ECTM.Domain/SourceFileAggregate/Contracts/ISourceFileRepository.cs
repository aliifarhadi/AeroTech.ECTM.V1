namespace AeroTech.ECTM.Domain.SourceFileAggregate.Contracts
{
    public interface ISourceFileRepository
    {
        Task AddAsync(SourceFile sourceFile, CancellationToken cancellationToken = default);

        Task<SourceFile?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SourceFile>> ListBySha256Async(IReadOnlyCollection<string> sha256s, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SourceFile>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

        Task<byte[]?> GetBytesAsync(long id, CancellationToken cancellationToken = default);
    }
}
