using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.ProcessingJobAggregate
{
    public sealed class ProcessingJobRepository : IProcessingJobRepository
    {
        private readonly EctmDbContext _dbContext;

        public ProcessingJobRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(ProcessingJob job, CancellationToken cancellationToken = default)
            => await _dbContext.ProcessingJobs.AddAsync(job, cancellationToken);

        public Task<ProcessingJob?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.ProcessingJobs.FirstOrDefaultAsync(job => job.Id == id, cancellationToken);

        public Task<ProcessingJob?> FindNextRunnableAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
            => _dbContext.ProcessingJobs
                .Where(job => job.State == ProcessingJobState.QUEUED
                              || (job.State == ProcessingJobState.RUNNING && job.LeaseExpiresAt < now))
                .OrderBy(job => job.CreatedAt)
                .ThenBy(job => job.Id)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<ProcessingJob?> FindQueuedAsync(ProcessingJobKind kind, long subjectId, CancellationToken cancellationToken = default)
        {
            var unsaved = _dbContext.ProcessingJobs.Local
                .Where(job => job.Kind == kind)
                .Where(job => job.SubjectId == subjectId)
                .FirstOrDefault(job => job.State == ProcessingJobState.QUEUED);

            if (unsaved is not null)
                return unsaved;

            return await _dbContext.ProcessingJobs
                .Where(job => job.Kind == kind)
                .Where(job => job.SubjectId == subjectId)
                .FirstOrDefaultAsync(job => job.State == ProcessingJobState.QUEUED, cancellationToken);
        }

        public Task<ProcessingJob?> FindLatestAsync(ProcessingJobKind kind, long subjectId, CancellationToken cancellationToken = default)
            => _dbContext.ProcessingJobs
                .AsNoTracking()
                .Where(job => job.Kind == kind)
                .Where(job => job.SubjectId == subjectId)
                .OrderByDescending(job => job.CreatedAt)
                .ThenByDescending(job => job.Id)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
