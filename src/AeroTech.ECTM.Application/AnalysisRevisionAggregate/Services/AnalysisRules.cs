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
        ConfigurationDocument? SnapshotDocument,
        SnapshotRulesContent? Snapshot,
        IReadOnlySet<string> ParameterCodes)
    {
        public bool AreUsedBy(AnalysisRevision revision, string engineAssignments)
            => revision.UsesInputs(QualityDocument.Id, PhaseDocument?.Id, SnapshotDocument?.Id, engineAssignments);
    }
}
