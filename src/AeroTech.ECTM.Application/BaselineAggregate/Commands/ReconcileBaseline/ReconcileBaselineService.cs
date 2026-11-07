using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.ReconcileBaseline
{
    public sealed class ReconcileBaselineService : IReconcileBaselineService
    {
        private readonly IBaselineRepository _baselines;
        private readonly BaselineEvaluationScheduler _evaluations;
        private readonly IUnitOfWork _unitOfWork;

        public ReconcileBaselineService(IBaselineRepository baselines, BaselineEvaluationScheduler evaluations, IUnitOfWork unitOfWork)
        {
            _baselines = baselines;
            _evaluations = evaluations;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeProcessingJobDto> ReconcileAsync(IReconcileBaselineCommand command, CancellationToken cancellationToken = default)
        {
            var baseline = await _baselines.FindAsync(command.BaselineId, cancellationToken)
                           ?? throw ExceptionFactory.BaselineNotFound(command.BaselineId);

            if (!baseline.IsEvaluated)
                throw ExceptionFactory.BaselineIsNotPublished(baseline.Id, baseline.State);

            var job = await _evaluations.ScheduleForEngineAsync(baseline.EngineId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BackofficeProcessingJobDto.From(job);
        }
    }
}
