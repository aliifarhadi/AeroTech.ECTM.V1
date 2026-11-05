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

            return await RulesOfAsync(quality, phase, cancellationToken);
        }

        public async Task<AnalysisRules> LoadChosenAsync(long? qualityRulesDocumentId, long? phaseRuleSetDocumentId, CancellationToken cancellationToken = default)
        {
            var quality = qualityRulesDocumentId is { } qualityId
                ? await PublishedAsync(qualityId, ConfigurationKind.QUALITY_RULES, cancellationToken)
                : await ActiveQualityRulesAsync(cancellationToken);
            var phase = phaseRuleSetDocumentId is { } phaseId
                ? await PublishedAsync(phaseId, ConfigurationKind.PHASE_RULESET, cancellationToken)
                : await _documents.FindActiveAsync(ConfigurationKind.PHASE_RULESET, cancellationToken);

            return await RulesOfAsync(quality, phase, cancellationToken);
        }

        public async Task<AnalysisRules> LoadOfRevisionAsync(AnalysisRevision revision, CancellationToken cancellationToken = default)
        {
            var quality = await DocumentAsync(revision.QualityRulesDocumentId, cancellationToken);
            var phase = revision.PhaseRuleSetDocumentId is { } phaseId ? await DocumentAsync(phaseId, cancellationToken) : null;

            return await RulesOfAsync(quality, phase, cancellationToken);
        }

        public async Task<AnalysisRules> LoadWithPhaseDraftAsync(AnalysisRevision? revision, ConfigurationDocument phaseDraft, CancellationToken cancellationToken = default)
        {
            var quality = revision is null
                ? await ActiveQualityRulesAsync(cancellationToken)
                : await DocumentAsync(revision.QualityRulesDocumentId, cancellationToken);

            return await RulesOfAsync(quality, phaseDraft, cancellationToken);
        }

        private async Task<AnalysisRules> RulesOfAsync(ConfigurationDocument quality, ConfigurationDocument? phase, CancellationToken cancellationToken)
        {
            var catalog = await _activeCatalog.LoadAsync(cancellationToken);
            var qualityValidation = QualityRulesValidator.Validate(quality.Content, catalog);

            if (!qualityValidation.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(quality.Id, qualityValidation.Issues.Count);

            var parameterCodes = FlightQualityInputLoader.CodesOf(qualityValidation.Rules!);

            if (phase is null)
                return new AnalysisRules(quality, qualityValidation.Rules!, null, null, parameterCodes);

            var phaseValidation = PhaseRuleSetValidator.Validate(phase.Content, catalog);

            if (!phaseValidation.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(phase.Id, phaseValidation.Issues.Count);

            parameterCodes.UnionWith(phaseValidation.ParameterCodes);

            return new AnalysisRules(quality, qualityValidation.Rules!, phase, phaseValidation.Rules, parameterCodes);
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

        private async Task<ConfigurationDocument> DocumentAsync(long documentId, CancellationToken cancellationToken)
            => await _documents.GetAsync(documentId, cancellationToken)
               ?? throw ExceptionFactory.ConfigurationDocumentNotFound(documentId);
    }
}
