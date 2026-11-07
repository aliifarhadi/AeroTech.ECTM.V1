using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services
{
    public sealed class BaselineEvaluationScheduler
    {
        private const int OneEngine = 1;

        private readonly IProcessingJobRepository _jobs;
        private readonly IBaselineRepository _baselines;
        private readonly IBaselineParameterFlightRepository _evaluations;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public BaselineEvaluationScheduler(
            IProcessingJobRepository jobs,
            IBaselineRepository baselines,
            IBaselineParameterFlightRepository evaluations,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _jobs = jobs;
            _baselines = baselines;
            _evaluations = evaluations;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<ProcessingJob> ScheduleForEngineAsync(long engineId, CancellationToken cancellationToken = default)
        {
            var queued = await _jobs.FindQueuedAsync(ProcessingJobKind.BASELINE_EVALUATION, engineId, cancellationToken);

            if (queued is not null)
                return queued;

            var job = ProcessingJob.Queue(_idGenerator.NewId(), ProcessingJobKind.BASELINE_EVALUATION, engineId, OneEngine, _clock.GetDateTime());

            await _jobs.AddAsync(job, cancellationToken);

            return job;
        }

        public async Task ScheduleForFlightAsync(Flight flight, CancellationToken cancellationToken = default)
        {
            var assignedEngineIds = flight.Engines
                .Where(engine => engine.EngineId is not null)
                .Select(engine => engine.EngineId!.Value);
            var evaluatedEngineIds = await _evaluations.ListEngineIdsWithCurrentForFlightAsync(flight.Id, cancellationToken);

            foreach (var engineId in assignedEngineIds.Union(evaluatedEngineIds))
            {
                if (await _baselines.AnyEvaluatedForEngineAsync(engineId, cancellationToken))
                    await ScheduleForEngineAsync(engineId, cancellationToken);
            }
        }
    }
}
