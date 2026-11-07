using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts
{
    public interface IProcessingJobRepository
    {
        Task AddAsync(ProcessingJob job, CancellationToken cancellationToken = default);

        Task<ProcessingJob?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<ProcessingJob?> FindNextRunnableAsync(DateTimeOffset now, CancellationToken cancellationToken = default);

        Task<ProcessingJob?> FindQueuedAsync(ProcessingJobKind kind, long subjectId, CancellationToken cancellationToken = default);

        Task<ProcessingJob?> FindLatestAsync(ProcessingJobKind kind, long subjectId, CancellationToken cancellationToken = default);
    }
}
