using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PublishBaseline
{
    public sealed class PublishBaselineService : IPublishBaselineService
    {
        private readonly IBaselineRepository _baselines;
        private readonly BaselineEvaluationScheduler _evaluations;
        private readonly BaselineDtoLoader _baselineDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActorResolver _actorResolver;
        private readonly IClock _clock;

        public PublishBaselineService(
            IBaselineRepository baselines,
            BaselineEvaluationScheduler evaluations,
            BaselineDtoLoader baselineDtos,
            IUnitOfWork unitOfWork,
            IActorResolver actorResolver,
            IClock clock)
        {
            _baselines = baselines;
            _evaluations = evaluations;
            _baselineDtos = baselineDtos;
            _unitOfWork = unitOfWork;
            _actorResolver = actorResolver;
            _clock = clock;
        }

        public async Task<BackofficeBaselinePublicationDto> PublishAsync(IPublishBaselineCommand command, CancellationToken cancellationToken = default)
        {
            var baseline = await _baselines.GetAsync(command.BaselineId, cancellationToken)
                           ?? throw ExceptionFactory.BaselineNotFound(command.BaselineId);

            baseline.Publish(command.ExpectedDraftRevision, _actorResolver.Resolve().Subject, _clock.GetDateTime());

            var job = await _evaluations.ScheduleForEngineAsync(baseline.EngineId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var baselineDto = await _baselineDtos.LoadAsync(baseline, cancellationToken);

            return new BackofficeBaselinePublicationDto(baselineDto, BackofficeProcessingJobDto.From(job));
        }
    }
}
