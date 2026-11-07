using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Arguments;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightReviewAggregate;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services
{
    public sealed class AnalysisRecorder
    {
        private readonly IAnalysisRevisionRepository _revisions;
        private readonly IFlightQualityRepository _qualities;
        private readonly IFlightPhaseAssessmentRepository _phases;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;
        private readonly IFlightReviewRepository _reviews;
        private readonly ISourceFileRepository _sourceFiles;
        private readonly BaselineEvaluationScheduler _baselineEvaluations;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public AnalysisRecorder(
            IAnalysisRevisionRepository revisions,
            IFlightQualityRepository qualities,
            IFlightPhaseAssessmentRepository phases,
            IFlightStabilityPointsRepository stabilityPoints,
            IFlightReviewRepository reviews,
            ISourceFileRepository sourceFiles,
            BaselineEvaluationScheduler baselineEvaluations,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _revisions = revisions;
            _qualities = qualities;
            _phases = phases;
            _stabilityPoints = stabilityPoints;
            _reviews = reviews;
            _sourceFiles = sourceFiles;
            _baselineEvaluations = baselineEvaluations;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<AnalysisRevision> OpenAsync(Flight flight, AnalysisRules rules, RevisionTrigger trigger, CancellationToken cancellationToken = default)
        {
            var sourceFile = await _sourceFiles.GetAsync(flight.SourceFileId, cancellationToken)
                             ?? throw ExceptionFactory.SourceFileIsUnreadable($"source file {flight.SourceFileId} is missing");
            var number = await _revisions.LastNumberAsync(flight.Id, cancellationToken) + 1;
            var args = new AnalysisRevisionArgs(
                flight.Id,
                sourceFile.Sha256,
                flight.Versions,
                rules.QualityDocument.Id,
                rules.QualityDocument.VersionNumber,
                rules.PhaseDocument?.Id,
                rules.PhaseDocument?.VersionNumber,
                rules.SnapshotDocument?.Id,
                rules.SnapshotDocument?.VersionNumber,
                flight.EngineAssignmentKey());
            var revision = AnalysisRevision.Open(_idGenerator.NewId(), number, trigger, args, _clock.GetDateTime());

            await _revisions.AddAsync(revision, cancellationToken);

            return revision;
        }

        public async Task CompleteAsync(Flight flight, AnalysisRevision revision, AnalysisRules rules, QualityInput input, CancellationToken cancellationToken = default)
        {
            var now = _clock.GetDateTime();

            await SupersedeCurrentAsync(flight.Id, now, cancellationToken);

            var quality = QualityEvaluator.Evaluate(rules.Quality, input);
            await _qualities.AddAsync(FlightQuality.Record(_idGenerator.NewId(), flight.Id, revision.Id, rules.QualityDocument.Id, rules.QualityDocument.VersionNumber, quality, now), cancellationToken);

            var phases = rules.Phase is { } phaseRules ? PhaseEvaluator.Evaluate(phaseRules, input, quality) : null;

            if (phases is not null)
                await _phases.AddAsync(FlightPhaseAssessment.Record(_idGenerator.NewId(), flight.Id, revision.Id, rules.PhaseDocument!.Id, rules.PhaseDocument.VersionNumber, phases, now), cancellationToken);

            if (rules.Snapshot is { } snapshotRules)
            {
                var evaluation = StabilityPointsEvaluator.Evaluate(snapshotRules, input, quality, phases);
                await _stabilityPoints.AddAsync(FlightStabilityPoints.Record(_idGenerator.NewId(), flight.Id, revision.Id, rules.SnapshotDocument!.Id, rules.SnapshotDocument.VersionNumber, evaluation, now), cancellationToken);
            }

            await _reviews.AddAsync(FlightReview.Open(_idGenerator.NewId(), flight.Id, revision.Id, now), cancellationToken);
            revision.Activate(now);
            flight.ReturnToReview();

            await _baselineEvaluations.ScheduleForFlightAsync(flight, cancellationToken);
        }

        private async Task SupersedeCurrentAsync(long flightId, DateTimeOffset now, CancellationToken cancellationToken)
        {
            var revision = await _revisions.GetCurrentAsync(flightId, cancellationToken);
            var quality = await _qualities.GetCurrentAsync(flightId, cancellationToken);
            var phases = await _phases.GetCurrentAsync(flightId, cancellationToken);
            var stabilityPoints = await _stabilityPoints.GetCurrentAsync(flightId, cancellationToken);
            var review = await _reviews.GetCurrentAsync(flightId, cancellationToken);

            revision?.Supersede(now);
            quality?.Supersede();
            phases?.Supersede();
            stabilityPoints?.Supersede();
            review?.Supersede();
        }
    }
}
