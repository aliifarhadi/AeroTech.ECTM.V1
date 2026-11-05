using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeEngine
{
    public sealed class RecomputeEngineService : IRecomputeEngineService
    {
        private readonly IEngineRepository _engines;
        private readonly IFlightRepository _flights;
        private readonly AnalysisRulesLoader _rules;
        private readonly RecomputeScheduler _scheduler;
        private readonly IUnitOfWork _unitOfWork;

        public RecomputeEngineService(
            IEngineRepository engines,
            IFlightRepository flights,
            AnalysisRulesLoader rules,
            RecomputeScheduler scheduler,
            IUnitOfWork unitOfWork)
        {
            _engines = engines;
            _flights = flights;
            _rules = rules;
            _scheduler = scheduler;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeEngineRecomputeDto> RecomputeAsync(IRecomputeEngineCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _engines.ExistsAsync(command.EngineId, cancellationToken))
                throw ExceptionFactory.EngineNotFound(command.EngineId);

            var rules = await _rules.LoadChosenAsync(command.QualityRulesDocumentId, command.PhaseRuleSetDocumentId, command.SnapshotRulesDocumentId, cancellationToken);
            var flights = await _flights.ListForEngineAsync(command.EngineId, FlightState.ACCEPTED, cancellationToken);
            var items = new List<BackofficeEngineRecomputeItemDto>();

            foreach (var flight in flights)
            {
                var schedule = await _scheduler.ScheduleAsync(flight, rules, RevisionTrigger.RECOMPUTE, cancellationToken);
                var revision = schedule.Revision is null ? null : BackofficeAnalysisRevisionDto.From(schedule.Revision);

                items.Add(new BackofficeEngineRecomputeItemDto(flight.Id, EnumValueDto.Of(schedule.Outcome), revision, schedule.ProcessingJobId));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var queued = items.Count(item => item.Outcome.Name == nameof(RecomputeOutcome.QUEUED));

            return new BackofficeEngineRecomputeDto(command.EngineId, flights.Count, queued, items);
        }
    }
}
