namespace AeroTech.ECTM.RestApi.V1.EngineInstallationAggregate.Requests
{
    public sealed record CreateInstallationRequest(
        long EngineId,
        long AircraftId,
        int Position,
        DateTimeOffset InstalledUtc,
        string Reason,
        string? Evidence);
}
