using AeroTech.ECTM.Application.ProcessingJobAggregate.Services;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services
{
    public sealed class BaselineEvaluationJobHandler : IProcessingJobHandler
    {
        private readonly EngineBaselineReconciler _reconciler;

        public BaselineEvaluationJobHandler(EngineBaselineReconciler reconciler) => _reconciler = reconciler;

        public ProcessingJobKind Kind => ProcessingJobKind.BASELINE_EVALUATION;

        public Task<IReadOnlyList<long>> PrepareAsync(long subjectId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<long>>([subjectId]);

        public Task ProcessAsync(long subjectId, long unitId, CancellationToken cancellationToken = default)
            => _reconciler.ReconcileAsync(unitId, cancellationToken);

        public Task AbandonAsync(long subjectId, long unitId, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task FinishAsync(long subjectId, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task FailAsync(long subjectId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
