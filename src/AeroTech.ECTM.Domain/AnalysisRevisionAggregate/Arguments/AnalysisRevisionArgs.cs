using AeroTech.ECTM.Domain._Shared;

namespace AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Arguments
{
    public sealed record AnalysisRevisionArgs(
        long FlightId,
        string SourceSha256,
        ConfigurationVersions Parsing,
        long QualityRulesDocumentId,
        int QualityRulesVersion,
        long? PhaseRuleSetDocumentId,
        int? PhaseRuleSetVersion);
}
