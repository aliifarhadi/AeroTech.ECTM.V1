using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Arguments;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine
{
    public sealed class UpdateEngineService : IUpdateEngineService
    {
        private readonly IEngineRepository _engines;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly IActorResolver _actorResolver;
        private readonly IGetEngineByIdService _getEngineById;

        public UpdateEngineService(
            IEngineRepository engines,
            IUnitOfWork unitOfWork,
            IClock clock,
            IActorResolver actorResolver,
            IGetEngineByIdService getEngineById)
        {
            _engines = engines;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _actorResolver = actorResolver;
            _getEngineById = getEngineById;
        }

        public async Task<BackofficeEngineDetailDto> UpdateAsync(IUpdateEngineCommand command, CancellationToken cancellationToken = default)
        {
            var engine = await _engines.GetAsync(command.EngineId, cancellationToken)
                         ?? throw ExceptionFactory.EngineNotFound(command.EngineId);

            var actor = _actorResolver.Resolve().Subject;
            var configuration = new EngineConfigurationArgs(command.VariantCode, command.ThrustRatingCode, command.ConfigurationEvidenceReference);

            engine.UpdateDetails(command.Status, configuration, actor, _clock.GetDateTime());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _getEngineById.ExecuteAsync(engine.Id, cancellationToken);
        }
    }
}
