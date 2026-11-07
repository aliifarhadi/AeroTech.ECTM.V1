using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.ECTM.Domain.SourceFileAggregate;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PrefillBaselineFromFile
{
    public sealed class PrefillBaselineFromFileService : IPrefillBaselineFromFileService
    {
        private readonly BaselineDtoLoader _baselineDtos;
        private readonly ShopVisitCheck _shopVisits;
        private readonly RecordingSetLoader _recordingSets;
        private readonly RecordingInspector _inspector;
        private readonly AnalysisRulesLoader _analysisRules;
        private readonly BaselineDefinitionsLoader _definitions;

        public PrefillBaselineFromFileService(
            BaselineDtoLoader baselineDtos,
            ShopVisitCheck shopVisits,
            RecordingSetLoader recordingSets,
            RecordingInspector inspector,
            AnalysisRulesLoader analysisRules,
            BaselineDefinitionsLoader definitions)
        {
            _baselineDtos = baselineDtos;
            _shopVisits = shopVisits;
            _recordingSets = recordingSets;
            _inspector = inspector;
            _analysisRules = analysisRules;
            _definitions = definitions;
        }

        public async Task<BackofficeBaselinePrefillDto> PrefillAsync(IPrefillBaselineFromFileCommand command, CancellationToken cancellationToken = default)
        {
            var serialNumber = await _baselineDtos.SerialNumberOfAsync(command.EngineId, cancellationToken)
                               ?? throw ExceptionFactory.EngineNotFound(command.EngineId);

            await _shopVisits.EnsureAsync(command.EngineId, command.ShopVisitEventId, cancellationToken);

            var points = await StabilityPointsOfAsync(command.File, cancellationToken);
            var definitions = await _definitions.LoadActiveAsync(cancellationToken);
            var snapshots = SnapshotReading.AllOf(points, command.EnginePosition)
                .Select(reading => BackofficePrefillSnapshotDto.From(reading, definitions))
                .ToList();

            return new BackofficeBaselinePrefillDto(
                command.EngineId,
                serialNumber,
                command.EnginePosition,
                command.ShopVisitEventId,
                command.File.Name,
                SourceFile.Sha256Of(command.File.Content),
                false,
                false,
                EnumValueDto.Of(ResultStatus.PROVISIONAL),
                EnumValueDto.OfNullable(points.NotEvaluableReason),
                definitions.CatalogDocument.Id,
                definitions.CatalogDocument.VersionNumber,
                definitions.SnapshotDocument.Id,
                definitions.SnapshotDocument.VersionNumber,
                snapshots);
        }

        private async Task<StabilityPointsResult> StabilityPointsOfAsync(UploadedFile file, CancellationToken cancellationToken)
        {
            var recordingSet = await _recordingSets.LoadActiveAsync(cancellationToken);
            var inspection = _inspector.Inspect(file.Name, file.Content, recordingSet);

            if (inspection.IsRejected)
                throw ExceptionFactory.BaselinePrefillFileRejected(inspection.RejectionCode, inspection.RejectionMessage);

            var rules = await _analysisRules.FindActiveAsync(cancellationToken)
                        ?? throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.QUALITY_RULES);

            if (rules.Phase is null)
                throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.PHASE_RULESET);

            if (rules.Snapshot is null)
                throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.SNAPSHOT_RULES);

            var input = QualityInput.From(inspection.Recording!);
            var quality = QualityEvaluator.Evaluate(rules.Quality, input);
            var phases = PhaseEvaluator.Evaluate(rules.Phase, input, quality);
            var evaluation = StabilityPointsEvaluator.Evaluate(rules.Snapshot, input, quality, phases);

            return evaluation.Points;
        }
    }
}
