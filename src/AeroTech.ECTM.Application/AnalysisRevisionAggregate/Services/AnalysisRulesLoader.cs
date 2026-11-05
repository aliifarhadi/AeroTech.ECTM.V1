using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Application.FlightQualityAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services
{
    public sealed class AnalysisRulesLoader
    {
        private readonly IConfigurationDocumentRepository _documents;
        private readonly ActiveCatalogLoader _activeCatalog;

        public AnalysisRulesLoader(IConfigurationDocumentRepository documents, ActiveCatalogLoader activeCatalog)
        {
            _documents = documents;
            _activeCatalog = activeCatalog;
        }

        public async Task<AnalysisRules?> FindActiveAsync(CancellationToken cancellationToken = default)
        {
            var quality = await _documents.FindActiveAsync(ConfigurationKind.QUALITY_RULES, cancellationToken);

            if (quality is null)
                return null;

            var phase = await _documents.FindActiveAsync(ConfigurationKind.PHASE_RULESET, cancellationToken);
            var snapshot = await _documents.FindActiveAsync(ConfigurationKind.SNAPSHOT_RULES, cancellationToken);

            return await RulesOfAsync(new ChosenDocuments(quality, phase, snapshot), cancellationToken);
        }

        public async Task<AnalysisRules> LoadChosenAsync(long? qualityRulesDocumentId, long? phaseRuleSetDocumentId, long? snapshotRulesDocumentId, CancellationToken cancellationToken = default)
        {
            var quality = qualityRulesDocumentId is { } qualityId
                ? await PublishedAsync(qualityId, ConfigurationKind.QUALITY_RULES, cancellationToken)
                : await ActiveQualityRulesAsync(cancellationToken);
            var phase = await ChosenOrActiveAsync(phaseRuleSetDocumentId, ConfigurationKind.PHASE_RULESET, cancellationToken);
            var snapshot = await ChosenOrActiveAsync(snapshotRulesDocumentId, ConfigurationKind.SNAPSHOT_RULES, cancellationToken);

            return await RulesOfAsync(new ChosenDocuments(quality, phase, snapshot), cancellationToken);
        }

        public async Task<AnalysisRules> LoadOfRevisionAsync(AnalysisRevision revision, CancellationToken cancellationToken = default)
        {
            var quality = await DocumentAsync(revision.QualityRulesDocumentId, cancellationToken);
            var phase = await OptionalDocumentAsync(revision.PhaseRuleSetDocumentId, cancellationToken);
            var snapshot = await OptionalDocumentAsync(revision.SnapshotRulesDocumentId, cancellationToken);

            return await RulesOfAsync(new ChosenDocuments(quality, phase, snapshot), cancellationToken);
        }

        public async Task<AnalysisRules> LoadWithPhaseDraftAsync(AnalysisRevision? revision, ConfigurationDocument phaseDraft, CancellationToken cancellationToken = default)
        {
            var quality = revision is null
                ? await ActiveQualityRulesAsync(cancellationToken)
                : await DocumentAsync(revision.QualityRulesDocumentId, cancellationToken);

            return await RulesOfAsync(new ChosenDocuments(quality, phaseDraft, null), cancellationToken);
        }

        private async Task<AnalysisRules> RulesOfAsync(ChosenDocuments documents, CancellationToken cancellationToken)
        {
            var catalog = await _activeCatalog.LoadAsync(cancellationToken);
            var quality = QualityRulesValidator.Validate(documents.Quality.Content, catalog);

            if (!quality.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(documents.Quality.Id, quality.Issues.Count);

            var parameterCodes = FlightQualityInputLoader.CodesOf(quality.Rules!);
            var rules = new AnalysisRules(documents.Quality, quality.Rules!, null, null, null, null, parameterCodes);

            if (documents.Phase is not { } phaseDocument)
                return rules;

            var phase = PhaseRuleSetValidator.Validate(phaseDocument.Content, catalog);

            if (!phase.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(phaseDocument.Id, phase.Issues.Count);

            parameterCodes.UnionWith(phase.ParameterCodes);
            rules = rules with { PhaseDocument = phaseDocument, Phase = phase.Rules };

            if (documents.Snapshot is not { } snapshotDocument)
                return rules;

            var snapshot = SnapshotRulesValidator.Validate(snapshotDocument.Content, catalog);

            if (!snapshot.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(snapshotDocument.Id, snapshot.Issues.Count);

            parameterCodes.UnionWith(snapshot.ParameterCodes);

            return rules with { SnapshotDocument = snapshotDocument, Snapshot = snapshot.Rules };
        }

        private async Task<ConfigurationDocument?> ChosenOrActiveAsync(long? documentId, ConfigurationKind kind, CancellationToken cancellationToken)
        {
            if (documentId is { } chosen)
                return await PublishedAsync(chosen, kind, cancellationToken);

            return await _documents.FindActiveAsync(kind, cancellationToken);
        }

        private async Task<ConfigurationDocument> ActiveQualityRulesAsync(CancellationToken cancellationToken)
            => await _documents.FindActiveAsync(ConfigurationKind.QUALITY_RULES, cancellationToken)
               ?? throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.QUALITY_RULES);

        private async Task<ConfigurationDocument> PublishedAsync(long documentId, ConfigurationKind kind, CancellationToken cancellationToken)
        {
            var document = await DocumentAsync(documentId, cancellationToken);

            if (document.Kind != kind || document.Status != ConfigurationDocumentStatus.PUBLISHED)
                throw ExceptionFactory.DocumentNotPublishedOfKind(documentId, kind);

            return document;
        }

        private async Task<ConfigurationDocument?> OptionalDocumentAsync(long? documentId, CancellationToken cancellationToken)
            => documentId is { } id ? await DocumentAsync(id, cancellationToken) : null;

        private async Task<ConfigurationDocument> DocumentAsync(long documentId, CancellationToken cancellationToken)
            => await _documents.GetAsync(documentId, cancellationToken)
               ?? throw ExceptionFactory.ConfigurationDocumentNotFound(documentId);

        private sealed record ChosenDocuments(ConfigurationDocument Quality, ConfigurationDocument? Phase, ConfigurationDocument? Snapshot);
    }
}
