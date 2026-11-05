using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services
{
    public sealed class RecomputeScheduler
    {
        private const int OneFlight = 1;

        private readonly IAnalysisRevisionRepository _revisions;
        private readonly IProcessingJobRepository _jobs;
        private readonly AnalysisRecorder _recorder;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public RecomputeScheduler(
            IAnalysisRevisionRepository revisions,
            IProcessingJobRepository jobs,
            AnalysisRecorder recorder,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _revisions = revisions;
            _jobs = jobs;
            _recorder = recorder;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<RecomputeSchedule> ScheduleAsync(Flight flight, AnalysisRules rules, RevisionTrigger trigger, CancellationToken cancellationToken = default)
        {
            var pending = await _revisions.FindPendingAsync(flight.Id, cancellationToken);

            if (pending is not null)
                return new RecomputeSchedule(RecomputeOutcome.ALREADY_PENDING, pending, pending.ProcessingJobId);

            var current = await _revisions.FindCurrentAsync(flight.Id, cancellationToken);

            if (current is not null && rules.AreUsedBy(current, flight.EngineAssignmentKey()))
                return new RecomputeSchedule(RecomputeOutcome.UNCHANGED, current, null);

            var revision = await _recorder.OpenAsync(flight, rules, trigger, cancellationToken);
            var job = ProcessingJob.Queue(_idGenerator.NewId(), ProcessingJobKind.RECOMPUTE, revision.Id, OneFlight, _clock.GetDateTime());

            revision.AssignJob(job.Id);
            await _jobs.AddAsync(job, cancellationToken);

            return new RecomputeSchedule(RecomputeOutcome.QUEUED, revision, job.Id);
        }
    }
}
