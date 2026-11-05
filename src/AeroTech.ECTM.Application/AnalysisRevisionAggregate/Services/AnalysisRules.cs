using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services
{
    public sealed record AnalysisRules(
        ConfigurationDocument QualityDocument,
        QualityRulesContent Quality,
        ConfigurationDocument? PhaseDocument,
        PhaseRuleSetContent? Phase,
        IReadOnlySet<string> ParameterCodes)
    {
        public bool AreUsedBy(AnalysisRevision revision) => revision.UsesVersions(QualityDocument.Id, PhaseDocument?.Id);
    }
}
