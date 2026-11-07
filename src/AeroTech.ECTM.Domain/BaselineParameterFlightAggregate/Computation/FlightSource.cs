namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation
{
    public sealed record FlightSource(
        long EngineId,
        long FlightId,
        DateOnly? FlightDate,
        string? SourceFileSha256,
        long AnalysisRevisionId,
        long FlightStabilityPointsId,
        long SnapshotRulesDocumentId,
        int SnapshotRulesVersion,
        int Position);
}
