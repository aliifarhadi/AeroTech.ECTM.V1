using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters
{
    public sealed class AdjustEngineLifeCountersService : IAdjustEngineLifeCountersService
    {
        private readonly IEngineRepository _engines;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;
        private readonly IActorResolver _actorResolver;
        private readonly IGetEngineByIdService _getEngineById;

        public AdjustEngineLifeCountersService(
            IEngineRepository engines,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock,
            IActorResolver actorResolver,
            IGetEngineByIdService getEngineById)
        {
            _engines = engines;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
            _actorResolver = actorResolver;
            _getEngineById = getEngineById;
        }

        public async Task<BackofficeEngineDetailDto> AdjustAsync(IAdjustEngineLifeCountersCommand command, CancellationToken cancellationToken = default)
        {
            var engine = await _engines.GetAsync(command.EngineId, cancellationToken)
                         ?? throw ExceptionFactory.EngineNotFound(command.EngineId);

            var actor = _actorResolver.Resolve().Subject;

            engine.AdjustLifeCounters(_idGenerator.NewId(), command.LifeCounters.ToArgs(), command.Reason, actor, _clock.GetDateTime());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _getEngineById.ExecuteAsync(engine.Id, cancellationToken);
        }
    }
}
