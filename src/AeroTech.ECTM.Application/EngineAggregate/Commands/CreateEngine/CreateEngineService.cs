using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById;
using AeroTech.ECTM.Application.EngineAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate;
using AeroTech.ECTM.Domain.EngineAggregate.Arguments;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine
{
    public sealed class CreateEngineService : ICreateEngineService
    {
        private readonly IEngineRepository _engines;
        private readonly IEngineModelRepository _engineModels;
        private readonly IEngineLock _engineLock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;
        private readonly IActorResolver _actorResolver;
        private readonly IGetEngineByIdService _getEngineById;

        public CreateEngineService(
            IEngineRepository engines,
            IEngineModelRepository engineModels,
            IEngineLock engineLock,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock,
            IActorResolver actorResolver,
            IGetEngineByIdService getEngineById)
        {
            _engines = engines;
            _engineModels = engineModels;
            _engineLock = engineLock;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
            _actorResolver = actorResolver;
            _getEngineById = getEngineById;
        }

        public async Task<BackofficeEngineDetailDto> CreateAsync(ICreateEngineCommand command, CancellationToken cancellationToken = default)
        {
            var engineModel = await _engineModels.GetAsync(command.EngineModelId, cancellationToken)
                              ?? throw ExceptionFactory.EngineModelNotFound(command.EngineModelId);

            var actor = _actorResolver.Resolve().Subject;
            var now = _clock.GetDateTime();
            var configuration = new EngineConfigurationArgs(command.VariantCode, command.ThrustRatingCode, command.ConfigurationEvidenceReference);

            var engine = Engine.Register(
                _idGenerator.NewId(),
                engineModel.Id,
                engineModel.Manufacturer,
                command.SerialNumber,
                command.Status,
                configuration,
                actor,
                now);

            if (command.LifeCounters is { } lifeCounters)
                engine.AdjustLifeCounters(_idGenerator.NewId(), lifeCounters.ToArgs(), Engine.InitialLifeCountersReason, actor, now);

            await using (await _engineLock.AcquireSerialAsync(engine.Manufacturer, engine.SerialNumber, cancellationToken))
            {
                if (await _engines.SerialExistsAsync(engine.Manufacturer, engine.SerialNumber, cancellationToken))
                    throw ExceptionFactory.EngineSerialAlreadyExists(engine.Manufacturer, engine.SerialNumber);

                await _engines.AddAsync(engine, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return await _getEngineById.ExecuteAsync(engine.Id, cancellationToken);
        }
    }
}
