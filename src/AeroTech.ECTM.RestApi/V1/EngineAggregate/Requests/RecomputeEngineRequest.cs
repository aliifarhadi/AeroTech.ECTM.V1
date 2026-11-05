namespace AeroTech.ECTM.RestApi.V1.EngineAggregate.Requests
{
    public sealed record RecomputeEngineRequest(long? QualityRulesDocumentId, long? PhaseRuleSetDocumentId, long? SnapshotRulesDocumentId);
}
