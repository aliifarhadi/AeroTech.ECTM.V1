using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight
{
    public sealed class RecomputeFlightService : IRecomputeFlightService
    {
        private readonly IFlightRepository _flights;
        private readonly AnalysisRulesLoader _rules;
        private readonly RecomputeScheduler _scheduler;
        private readonly PhasePreviewer _previewer;
        private readonly IUnitOfWork _unitOfWork;

        public RecomputeFlightService(
            IFlightRepository flights,
            AnalysisRulesLoader rules,
            RecomputeScheduler scheduler,
            PhasePreviewer previewer,
            IUnitOfWork unitOfWork)
        {
            _flights = flights;
            _rules = rules;
            _scheduler = scheduler;
            _previewer = previewer;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeRecomputeDto> RecomputeAsync(IRecomputeFlightCommand command, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(command.FlightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(command.FlightId);
            var rules = await _rules.LoadChosenAsync(command.QualityRulesDocumentId, command.PhaseRuleSetDocumentId, command.SnapshotRulesDocumentId, cancellationToken);

            if (command.DryRun)
            {
                var preview = await _previewer.PreviewAsync(flight, rules, cancellationToken);

                return new BackofficeRecomputeDto(flight.Id, true, preview, null, null);
            }

            var schedule = await _scheduler.ScheduleAsync(flight, rules, RevisionTrigger.RECOMPUTE, cancellationToken);

            if (schedule.Outcome == RecomputeOutcome.ALREADY_PENDING)
                throw ExceptionFactory.RecomputeAlreadyPending(flight.Id, schedule.Revision!.Number);

            if (schedule.Outcome == RecomputeOutcome.UNCHANGED)
                throw ExceptionFactory.RecomputeChangesNothing(schedule.Revision!.Number, flight.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BackofficeRecomputeDto(flight.Id, false, null, BackofficeAnalysisRevisionDto.From(schedule.Revision!), schedule.ProcessingJobId);
        }
    }
}
