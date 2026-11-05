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
        private readonly ISourceFileRepository _sourceFiles;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public AnalysisRecorder(
            IAnalysisRevisionRepository revisions,
            IFlightQualityRepository qualities,
            IFlightPhaseAssessmentRepository phases,
            ISourceFileRepository sourceFiles,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _revisions = revisions;
            _qualities = qualities;
            _phases = phases;
            _sourceFiles = sourceFiles;
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
                rules.PhaseDocument?.VersionNumber);
            var revision = AnalysisRevision.Open(_idGenerator.NewId(), number, trigger, args, _clock.GetDateTime());

            await _revisions.AddAsync(revision, cancellationToken);

            return revision;
        }

        public async Task CompleteAsync(AnalysisRevision revision, AnalysisRules rules, QualityInput input, CancellationToken cancellationToken = default)
        {
            var now = _clock.GetDateTime();
            var quality = QualityEvaluator.Evaluate(rules.Quality, input);

            await SupersedeCurrentAsync(revision.FlightId, now, cancellationToken);
            await _qualities.AddAsync(FlightQuality.Record(_idGenerator.NewId(), revision.FlightId, revision.Id, rules.QualityDocument.Id, rules.QualityDocument.VersionNumber, quality, now), cancellationToken);

            if (rules.Phase is { } phaseRules)
            {
                var phases = PhaseEvaluator.Evaluate(phaseRules, input, quality);
                var assessment = FlightPhaseAssessment.Record(_idGenerator.NewId(), revision.FlightId, revision.Id, rules.PhaseDocument!.Id, rules.PhaseDocument.VersionNumber, phases, now);

                await _phases.AddAsync(assessment, cancellationToken);
            }

            revision.Activate(now);
        }

        private async Task SupersedeCurrentAsync(long flightId, DateTimeOffset now, CancellationToken cancellationToken)
        {
            var revision = await _revisions.GetCurrentAsync(flightId, cancellationToken);
            var quality = await _qualities.GetCurrentAsync(flightId, cancellationToken);
            var phases = await _phases.GetCurrentAsync(flightId, cancellationToken);

            revision?.Supersede(now);
            quality?.Supersede();
            phases?.Supersede();
        }
    }
}
