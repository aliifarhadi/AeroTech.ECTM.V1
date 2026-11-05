namespace AeroTech.ECTM.RestApi.V1.FlightAggregate.Requests
{
    public sealed record RecomputeFlightRequest(long? QualityRulesDocumentId, long? PhaseRuleSetDocumentId, long? SnapshotRulesDocumentId, bool? DryRun);
}
