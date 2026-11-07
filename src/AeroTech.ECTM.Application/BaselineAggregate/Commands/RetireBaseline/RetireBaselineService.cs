using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.RetireBaseline
{
    public sealed class RetireBaselineService : IRetireBaselineService
    {
        private readonly IBaselineRepository _baselines;
        private readonly IBaselinePrimarySelectionRepository _primarySelections;
        private readonly BaselineDtoLoader _baselineDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActorResolver _actorResolver;
        private readonly IClock _clock;

        public RetireBaselineService(
            IBaselineRepository baselines,
            IBaselinePrimarySelectionRepository primarySelections,
            BaselineDtoLoader baselineDtos,
            IUnitOfWork unitOfWork,
            IActorResolver actorResolver,
            IClock clock)
        {
            _baselines = baselines;
            _primarySelections = primarySelections;
            _baselineDtos = baselineDtos;
            _unitOfWork = unitOfWork;
            _actorResolver = actorResolver;
            _clock = clock;
        }

        public async Task<BackofficeBaselineDto> RetireAsync(IRetireBaselineCommand command, CancellationToken cancellationToken = default)
        {
            var baseline = await _baselines.GetAsync(command.BaselineId, cancellationToken)
                           ?? throw ExceptionFactory.BaselineNotFound(command.BaselineId);

            baseline.Retire(command.Reason, _actorResolver.Resolve().Subject, _clock.GetDateTime());

            var primarySelections = await _primarySelections.ListForBaselineAsync(baseline.Id, cancellationToken);

            foreach (var selection in primarySelections)
                _primarySelections.Remove(selection);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _baselineDtos.LoadAsync(baseline, cancellationToken);
        }
    }
}
