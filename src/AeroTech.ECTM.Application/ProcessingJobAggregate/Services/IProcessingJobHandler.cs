using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Services
{
    public interface IProcessingJobHandler
    {
        ProcessingJobKind Kind { get; }

        Task<IReadOnlyList<long>> PrepareAsync(long subjectId, CancellationToken cancellationToken = default);

        Task ProcessAsync(long subjectId, long unitId, CancellationToken cancellationToken = default);

        Task AbandonAsync(long subjectId, long unitId, CancellationToken cancellationToken = default);

        Task FinishAsync(long subjectId, CancellationToken cancellationToken = default);

        Task FailAsync(long subjectId, CancellationToken cancellationToken = default);
    }
}
