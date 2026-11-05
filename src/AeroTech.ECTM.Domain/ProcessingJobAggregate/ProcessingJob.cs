using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ProcessingJobAggregate
{
    public sealed class ProcessingJob : AggregateRoot<long>
    {
        private ProcessingJob()
        {
        }

        private ProcessingJob(long id, ProcessingJobKind kind, long subjectId, int total, DateTimeOffset createdAt)
        {
            Id = id;
            Kind = kind;
            SubjectId = subjectId;
            Total = total;
            State = ProcessingJobState.QUEUED;
            CreatedAt = createdAt;
        }

        public ProcessingJobKind Kind { get; private set; }

        public long SubjectId { get; private set; }

        public ProcessingJobState State { get; private set; }

        public int Processed { get; private set; }

        public int Total { get; private set; }

        public string? ErrorCode { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? CompletedAt { get; private set; }

        public DateTimeOffset? LeaseExpiresAt { get; private set; }

        public static ProcessingJob Queue(long id, ProcessingJobKind kind, long subjectId, int total, DateTimeOffset now)
            => new(id, kind, subjectId, total, now);

        public void Start(DateTimeOffset now, TimeSpan lease)
        {
            State = ProcessingJobState.RUNNING;
            LeaseExpiresAt = now + lease;
        }

        public void ReportProgress(int processed, DateTimeOffset now, TimeSpan lease)
        {
            Processed = processed;
            LeaseExpiresAt = now + lease;
        }

        public void Complete(DateTimeOffset now)
        {
            State = ProcessingJobState.COMPLETED;
            CompletedAt = now;
            LeaseExpiresAt = null;
        }

        public void Fail(string errorCode, DateTimeOffset now)
        {
            State = ProcessingJobState.FAILED;
            ErrorCode = errorCode;
            CompletedAt = now;
            LeaseExpiresAt = null;
        }
    }
}
