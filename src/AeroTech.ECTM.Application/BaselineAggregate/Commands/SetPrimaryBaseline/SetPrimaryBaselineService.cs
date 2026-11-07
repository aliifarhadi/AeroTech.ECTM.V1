using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Queries.GetEngineBaselines;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate;
using AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.SetPrimaryBaseline
{
    public sealed class SetPrimaryBaselineService : ISetPrimaryBaselineService
    {
        private readonly IBaselineRepository _baselines;
        private readonly IBaselinePrimarySelectionRepository _primarySelections;
        private readonly IGetEngineBaselinesService _engineBaselines;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IActorResolver _actorResolver;
        private readonly IClock _clock;

        public SetPrimaryBaselineService(
            IBaselineRepository baselines,
            IBaselinePrimarySelectionRepository primarySelections,
            IGetEngineBaselinesService engineBaselines,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IActorResolver actorResolver,
            IClock clock)
        {
            _baselines = baselines;
            _primarySelections = primarySelections;
            _engineBaselines = engineBaselines;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _actorResolver = actorResolver;
            _clock = clock;
        }

        public async Task<BackofficeEngineBaselinesDto> SetAsync(ISetPrimaryBaselineCommand command, CancellationToken cancellationToken = default)
        {
            if (command.BaselineParameterId is { } baselineParameterId)
                await SelectAsync(command, baselineParameterId, cancellationToken);
            else
                await ClearAsync(command, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _engineBaselines.ExecuteAsync(command.EngineId, cancellationToken);
        }

        private async Task SelectAsync(ISetPrimaryBaselineCommand command, long baselineParameterId, CancellationToken cancellationToken)
        {
            var baseline = await _baselines.GetByParameterIdAsync(baselineParameterId, cancellationToken)
                           ?? throw ExceptionFactory.BaselineParameterNotFound(baselineParameterId);

            if (baseline.EngineId != command.EngineId)
                throw ExceptionFactory.BaselineEngineMismatch(baselineParameterId, baseline.EngineId, command.EngineId);

            var parameter = baseline.ParameterOf(baselineParameterId);
            var current = await _primarySelections.GetAsync(command.EngineId, parameter.SnapshotKind, parameter.Selector, parameter.ParameterCode, cancellationToken);

            EnsureExpected(command.ExpectedCurrentPrimaryId, current);

            var actor = _actorResolver.Resolve().Subject;
            var now = _clock.GetDateTime();

            if (current is null)
                await _primarySelections.AddAsync(BaselinePrimarySelection.Select(_idGenerator.NewId(), baseline, parameter, actor, now), cancellationToken);
            else
                current.PointTo(baseline, parameter, actor, now);
        }

        private async Task ClearAsync(ISetPrimaryBaselineCommand command, CancellationToken cancellationToken)
        {
            var current = await _primarySelections.GetAsync(command.EngineId, command.SnapshotKind!.Value, command.Selector, command.ParameterCode!, cancellationToken);

            EnsureExpected(command.ExpectedCurrentPrimaryId, current);

            if (current is not null)
                _primarySelections.Remove(current);
        }

        private static void EnsureExpected(long? expectedCurrentPrimaryId, BaselinePrimarySelection? current)
        {
            var currentPrimaryId = current?.BaselineParameterId;

            if (expectedCurrentPrimaryId != currentPrimaryId)
                throw ExceptionFactory.BaselinePrimaryChanged(expectedCurrentPrimaryId?.ToString() ?? "none", currentPrimaryId?.ToString() ?? "none");
        }
    }
}
