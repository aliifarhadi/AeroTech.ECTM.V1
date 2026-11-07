using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CreateBaseline
{
    public sealed class CreateBaselineService : ICreateBaselineService
    {
        private readonly IBaselineRepository _baselines;
        private readonly IEngineRepository _engines;
        private readonly BaselineDefinitionsLoader _definitions;
        private readonly ShopVisitCheck _shopVisits;
        private readonly BaselineDtoLoader _baselineDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IActorResolver _actorResolver;
        private readonly IClock _clock;

        public CreateBaselineService(
            IBaselineRepository baselines,
            IEngineRepository engines,
            BaselineDefinitionsLoader definitions,
            ShopVisitCheck shopVisits,
            BaselineDtoLoader baselineDtos,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IActorResolver actorResolver,
            IClock clock)
        {
            _baselines = baselines;
            _engines = engines;
            _definitions = definitions;
            _shopVisits = shopVisits;
            _baselineDtos = baselineDtos;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _actorResolver = actorResolver;
            _clock = clock;
        }

        public async Task<BackofficeBaselineDto> CreateAsync(ICreateBaselineCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _engines.ExistsAsync(command.EngineId, cancellationToken))
                throw ExceptionFactory.EngineNotFound(command.EngineId);

            await _shopVisits.EnsureAsync(command.EngineId, command.ShopVisitEventId, cancellationToken);

            var definitions = await _definitions.LoadActiveAsync(cancellationToken);
            var parameters = BaselineDraftBuilder.ParametersOf(command.Parameters, definitions);
            var args = BaselineDraftBuilder.ArgsOf(command);
            var actor = _actorResolver.Resolve().Subject;

            var baseline = Baseline.Draft(_idGenerator.NewId(), command.EngineId, args, parameters, _idGenerator, actor, _clock.GetDateTime());

            await _baselines.AddAsync(baseline, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _baselineDtos.LoadAsync(baseline, cancellationToken);
        }
    }
}
