using AeroTech.ECTM.Application.FlightQualityAggregate.Services;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services
{
    public sealed class RecomputeJobHandler : IProcessingJobHandler
    {
        public const string UnitFailed = "UNIT_FAILED";
        public const string JobFailed = "JOB_FAILED";

        private readonly IAnalysisRevisionRepository _revisions;
        private readonly IFlightRepository _flights;
        private readonly AnalysisRulesLoader _rules;
        private readonly FlightQualityInputLoader _inputs;
        private readonly AnalysisRecorder _recorder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public RecomputeJobHandler(
            IAnalysisRevisionRepository revisions,
            IFlightRepository flights,
            AnalysisRulesLoader rules,
            FlightQualityInputLoader inputs,
            AnalysisRecorder recorder,
            IUnitOfWork unitOfWork,
            IClock clock)
        {
            _revisions = revisions;
            _flights = flights;
            _rules = rules;
            _inputs = inputs;
            _recorder = recorder;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public ProcessingJobKind Kind => ProcessingJobKind.RECOMPUTE;

        public async Task<IReadOnlyList<long>> PrepareAsync(long subjectId, CancellationToken cancellationToken = default)
        {
            var revision = await RevisionOfAsync(subjectId, cancellationToken);

            return revision.Status == RevisionStatus.PENDING ? [revision.Id] : [];
        }

        public async Task ProcessAsync(long subjectId, long unitId, CancellationToken cancellationToken = default)
        {
            var revision = await RevisionOfAsync(unitId, cancellationToken);

            if (revision.Status != RevisionStatus.PENDING)
                return;

            var flight = await _flights.GetAsync(revision.FlightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(revision.FlightId);
            var rules = await _rules.LoadOfRevisionAsync(revision, cancellationToken);
            var input = await _inputs.LoadAsync(flight, rules.ParameterCodes, cancellationToken);

            await _recorder.CompleteAsync(revision, rules, input, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public Task AbandonAsync(long subjectId, long unitId, CancellationToken cancellationToken = default)
            => FailPendingAsync(unitId, UnitFailed, cancellationToken);

        public Task FinishAsync(long subjectId, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task FailAsync(long subjectId, CancellationToken cancellationToken = default)
            => FailPendingAsync(subjectId, JobFailed, cancellationToken);

        private async Task FailPendingAsync(long revisionId, string failureCode, CancellationToken cancellationToken)
        {
            var revision = await RevisionOfAsync(revisionId, cancellationToken);

            if (revision.Status != RevisionStatus.PENDING)
                return;

            revision.Fail(failureCode, _clock.GetDateTime());

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<AnalysisRevision> RevisionOfAsync(long revisionId, CancellationToken cancellationToken)
            => await _revisions.GetAsync(revisionId, cancellationToken)
               ?? throw ExceptionFactory.ProcessingJobNotFound(revisionId);
    }
}
