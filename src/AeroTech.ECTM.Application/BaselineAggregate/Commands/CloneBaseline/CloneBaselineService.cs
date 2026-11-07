using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CloneBaseline
{
    public sealed class CloneBaselineService : ICloneBaselineService
    {
        private readonly IBaselineRepository _baselines;
        private readonly BaselineDtoLoader _baselineDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IActorResolver _actorResolver;
        private readonly IClock _clock;

        public CloneBaselineService(
            IBaselineRepository baselines,
            BaselineDtoLoader baselineDtos,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IActorResolver actorResolver,
            IClock clock)
        {
            _baselines = baselines;
            _baselineDtos = baselineDtos;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _actorResolver = actorResolver;
            _clock = clock;
        }

        public async Task<BackofficeBaselineDto> CloneAsync(ICloneBaselineCommand command, CancellationToken cancellationToken = default)
        {
            var source = await _baselines.FindAsync(command.BaselineId, cancellationToken)
                         ?? throw ExceptionFactory.BaselineNotFound(command.BaselineId);

            var lastRevision = await _baselines.LastRevisionNumberAsync(source.LineageId, cancellationToken);
            var actor = _actorResolver.Resolve().Subject;

            var clone = source.CloneAsDraft(_idGenerator.NewId(), lastRevision + 1, _idGenerator, actor, _clock.GetDateTime());

            await _baselines.AddAsync(clone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _baselineDtos.LoadAsync(clone, cancellationToken);
        }
    }
}
