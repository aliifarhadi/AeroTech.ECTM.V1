using AeroTech.ECTM.Application.AcceptanceTests.Fakes;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Services;
using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Imports;

public sealed class ProcessingJobRunnerTests
{
    private const long SubjectId = 99;

    private readonly FixedClock _clock = new();

    [Fact]
    public async Task A_failed_unit_is_abandoned_and_the_job_still_completes_with_progress()
    {
        var jobs = new InMemoryJobs();
        var handler = new RecordingHandler(failingUnit: 2);
        var job = ProcessingJob.Queue(1, ProcessingJobKind.IMPORT_COMMIT, SubjectId, 3, _clock.Now);
        jobs.Jobs.Add(job);

        var runner = RunnerWith(jobs, handler);

        Assert.True(await runner.RunNextAsync());
        Assert.False(await runner.RunNextAsync());

        Assert.Equal(ProcessingJobState.COMPLETED, job.State);
        Assert.Equal(3, job.Processed);
        Assert.Equal([1L, 3L], handler.Processed);
        Assert.Equal([2L], handler.Abandoned);
        Assert.True(handler.Finished);
        Assert.False(handler.Failed);
    }

    [Fact]
    public async Task A_failure_outside_the_units_fails_the_job_and_the_subject()
    {
        var jobs = new InMemoryJobs();
        var handler = new RecordingHandler(failingUnit: null) { FailOnFinish = true };
        var job = ProcessingJob.Queue(1, ProcessingJobKind.IMPORT_COMMIT, SubjectId, 3, _clock.Now);
        jobs.Jobs.Add(job);

        await RunnerWith(jobs, handler).RunNextAsync();

        Assert.Equal(ProcessingJobState.FAILED, job.State);
        Assert.Equal(ProcessingJobRunner.UnexpectedFailure, job.ErrorCode);
        Assert.True(handler.Failed);
    }

    private ProcessingJobRunner RunnerWith(InMemoryJobs jobs, RecordingHandler handler)
    {
        var services = new ServiceCollection()
            .AddSingleton<IProcessingJobRepository>(jobs)
            .AddSingleton<IUnitOfWork>(jobs)
            .AddSingleton<IClock>(_clock)
            .AddSingleton<IProcessingJobHandler>(handler)
            .BuildServiceProvider();

        return new ProcessingJobRunner(services.GetRequiredService<IServiceScopeFactory>(), Options.Create(new ProcessingJobOptions()), NullLogger<ProcessingJobRunner>.Instance);
    }

    private sealed class InMemoryJobs : IProcessingJobRepository, IUnitOfWork
    {
        public List<ProcessingJob> Jobs { get; } = new();

        public Task AddAsync(ProcessingJob job, CancellationToken cancellationToken = default)
        {
            Jobs.Add(job);
            return Task.CompletedTask;
        }

        public Task<ProcessingJob?> GetAsync(long id, CancellationToken cancellationToken = default)
            => Task.FromResult(Jobs.FirstOrDefault(job => job.Id == id));

        public Task<ProcessingJob?> FindNextRunnableAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
            => Task.FromResult(Jobs.FirstOrDefault(job => job.State == ProcessingJobState.QUEUED));

        public Task<ProcessingJob?> FindQueuedAsync(ProcessingJobKind kind, long subjectId, CancellationToken cancellationToken = default)
            => Task.FromResult(Jobs.Where(job => job.Kind == kind).Where(job => job.SubjectId == subjectId).FirstOrDefault(job => job.State == ProcessingJobState.QUEUED));

        public Task<ProcessingJob?> FindLatestAsync(ProcessingJobKind kind, long subjectId, CancellationToken cancellationToken = default)
            => Task.FromResult(Jobs.Where(job => job.Kind == kind).LastOrDefault(job => job.SubjectId == subjectId));

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }

    private sealed class RecordingHandler(long? failingUnit) : IProcessingJobHandler
    {
        public List<long> Processed { get; } = new();

        public List<long> Abandoned { get; } = new();

        public bool Finished { get; private set; }

        public bool Failed { get; private set; }

        public bool FailOnFinish { get; init; }

        public ProcessingJobKind Kind => ProcessingJobKind.IMPORT_COMMIT;

        public Task<IReadOnlyList<long>> PrepareAsync(long subjectId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<long>>([1, 2, 3]);

        public Task ProcessAsync(long subjectId, long unitId, CancellationToken cancellationToken = default)
        {
            if (unitId == failingUnit)
                throw new InvalidOperationException("unit failure for the test");

            Processed.Add(unitId);
            return Task.CompletedTask;
        }

        public Task AbandonAsync(long subjectId, long unitId, CancellationToken cancellationToken = default)
        {
            Abandoned.Add(unitId);
            return Task.CompletedTask;
        }

        public Task FinishAsync(long subjectId, CancellationToken cancellationToken = default)
        {
            if (FailOnFinish)
                throw new InvalidOperationException("finish failure for the test");

            Finished = true;
            return Task.CompletedTask;
        }

        public Task FailAsync(long subjectId, CancellationToken cancellationToken = default)
        {
            Failed = true;
            return Task.CompletedTask;
        }
    }
}
