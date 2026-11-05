using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight
{
    public sealed class RecomputeFlightService : IRecomputeFlightService
    {
        private const int OneFlight = 1;

        private readonly IFlightRepository _flights;
        private readonly IAnalysisRevisionRepository _revisions;
        private readonly IProcessingJobRepository _jobs;
        private readonly AnalysisRulesLoader _rules;
        private readonly AnalysisRecorder _recorder;
        private readonly PhasePreviewer _previewer;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public RecomputeFlightService(
            IFlightRepository flights,
            IAnalysisRevisionRepository revisions,
            IProcessingJobRepository jobs,
            AnalysisRulesLoader rules,
            AnalysisRecorder recorder,
            PhasePreviewer previewer,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _flights = flights;
            _revisions = revisions;
            _jobs = jobs;
            _rules = rules;
            _recorder = recorder;
            _previewer = previewer;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<BackofficeRecomputeDto> RecomputeAsync(IRecomputeFlightCommand command, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(command.FlightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(command.FlightId);
            var rules = await _rules.LoadChosenAsync(command.QualityRulesDocumentId, command.PhaseRuleSetDocumentId, cancellationToken);

            if (command.DryRun)
            {
                var preview = await _previewer.PreviewAsync(flight, rules, cancellationToken);

                return new BackofficeRecomputeDto(flight.Id, true, preview, null, null);
            }

            await EnsureSomethingChangesAsync(flight, rules, cancellationToken);

            return await QueueAsync(flight, rules, cancellationToken);
        }

        private async Task EnsureSomethingChangesAsync(Flight flight, AnalysisRules rules, CancellationToken cancellationToken)
        {
            var pending = await _revisions.FindPendingAsync(flight.Id, cancellationToken);

            if (pending is not null)
                throw ExceptionFactory.RecomputeAlreadyPending(flight.Id, pending.Number);

            var current = await _revisions.FindCurrentAsync(flight.Id, cancellationToken);

            if (current is not null && rules.AreUsedBy(current))
                throw ExceptionFactory.RecomputeChangesNothing(current.Number, flight.Id);
        }

        private async Task<BackofficeRecomputeDto> QueueAsync(Flight flight, AnalysisRules rules, CancellationToken cancellationToken)
        {
            var revision = await _recorder.OpenAsync(flight, rules, RevisionTrigger.RECOMPUTE, cancellationToken);
            var job = ProcessingJob.Queue(_idGenerator.NewId(), ProcessingJobKind.RECOMPUTE, revision.Id, OneFlight, _clock.GetDateTime());

            revision.AssignJob(job.Id);

            await _jobs.AddAsync(job, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BackofficeRecomputeDto(flight.Id, false, null, BackofficeAnalysisRevisionDto.From(revision), job.Id);
        }
    }
}
