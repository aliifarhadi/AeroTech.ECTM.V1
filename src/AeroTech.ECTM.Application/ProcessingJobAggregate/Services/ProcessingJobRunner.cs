using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Services
{
    public sealed class ProcessingJobRunner : IProcessingJobRunner
    {
        public const string UnexpectedFailure = "UNEXPECTED_FAILURE";

        private const int MaxUnitAttempts = 2;

        private readonly IServiceScopeFactory _scopes;
        private readonly TimeSpan _lease;
        private readonly ILogger<ProcessingJobRunner> _logger;

        public ProcessingJobRunner(IServiceScopeFactory scopes, IOptions<ProcessingJobOptions> options, ILogger<ProcessingJobRunner> logger)
        {
            _scopes = scopes;
            _lease = TimeSpan.FromMinutes(options.Value.LeaseMinutes);
            _logger = logger;
        }

        public async Task<bool> RunNextAsync(CancellationToken cancellationToken = default)
        {
            JobClaim? claim;

            try
            {
                claim = await ClaimAsync(cancellationToken);
            }
            catch (BusinessException exception) when (exception.Code == ExceptionFactory.ResourceChangedConcurrentlyCode)
            {
                return true;
            }

            if (claim is null)
                return false;

            try
            {
                await ExecuteAsync(claim, cancellationToken);
                await UpdateJobAsync(claim, (job, now) => job.Complete(now), cancellationToken);
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(exception, "Processing job {JobId} of kind {Kind} failed.", claim.Id, claim.Kind);

                await WithHandlerAsync(claim, handler => handler.FailAsync(claim.SubjectId, cancellationToken));
                await UpdateJobAsync(claim, (job, now) => job.Fail(UnexpectedFailure, now), cancellationToken);
            }

            return true;
        }

        private async Task<JobClaim?> ClaimAsync(CancellationToken cancellationToken)
        {
            await using var scope = _scopes.CreateAsyncScope();

            var services = scope.ServiceProvider;
            var now = services.GetRequiredService<IClock>().GetDateTime();
            var job = await services.GetRequiredService<IProcessingJobRepository>().FindNextRunnableAsync(now, cancellationToken);

            if (job is null)
                return null;

            job.Start(now, _lease);

            await services.GetRequiredService<IUnitOfWork>().SaveChangesAsync(cancellationToken);

            return new JobClaim(job.Id, job.Kind, job.SubjectId, job.Total);
        }

        private async Task ExecuteAsync(JobClaim claim, CancellationToken cancellationToken)
        {
            IReadOnlyList<long> units = [];

            await WithHandlerAsync(claim, async handler => units = await handler.PrepareAsync(claim.SubjectId, cancellationToken));

            var processed = claim.Total - units.Count;

            await ReportAsync(claim, processed, cancellationToken);

            foreach (var unit in units)
            {
                await RunUnitAsync(claim, unit, cancellationToken);

                processed++;

                await ReportAsync(claim, processed, cancellationToken);
            }

            await WithHandlerAsync(claim, handler => handler.FinishAsync(claim.SubjectId, cancellationToken));
        }

        private async Task RunUnitAsync(JobClaim claim, long unit, CancellationToken cancellationToken)
        {
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    await WithHandlerAsync(claim, handler => handler.ProcessAsync(claim.SubjectId, unit, cancellationToken));

                    return;
                }
                catch (BusinessException exception) when (exception.Code == ExceptionFactory.ResourceChangedConcurrentlyCode && attempt < MaxUnitAttempts)
                {
                    _logger.LogInformation("Unit {Unit} of job {JobId} met a concurrent change and is retried.", unit, claim.Id);
                }
                catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(exception, "Unit {Unit} of job {JobId} failed.", unit, claim.Id);

                    await WithHandlerAsync(claim, handler => handler.AbandonAsync(claim.SubjectId, unit, cancellationToken));

                    return;
                }
            }
        }

        private Task ReportAsync(JobClaim claim, int processed, CancellationToken cancellationToken)
            => UpdateJobAsync(claim, (job, now) => job.ReportProgress(processed, now, _lease), cancellationToken);

        private async Task UpdateJobAsync(JobClaim claim, Action<ProcessingJob, DateTimeOffset> update, CancellationToken cancellationToken)
        {
            await using var scope = _scopes.CreateAsyncScope();

            var services = scope.ServiceProvider;
            var job = await services.GetRequiredService<IProcessingJobRepository>().GetAsync(claim.Id, cancellationToken)
                      ?? throw ExceptionFactory.ProcessingJobNotFound(claim.Id);

            update(job, services.GetRequiredService<IClock>().GetDateTime());

            await services.GetRequiredService<IUnitOfWork>().SaveChangesAsync(cancellationToken);
        }

        private async Task WithHandlerAsync(JobClaim claim, Func<IProcessingJobHandler, Task> work)
        {
            await using var scope = _scopes.CreateAsyncScope();

            var handler = scope.ServiceProvider
                .GetServices<IProcessingJobHandler>()
                .Single(candidate => candidate.Kind == claim.Kind);

            await work(handler);
        }

        private sealed record JobClaim(long Id, ProcessingJobKind Kind, long SubjectId, int Total);
    }
}
