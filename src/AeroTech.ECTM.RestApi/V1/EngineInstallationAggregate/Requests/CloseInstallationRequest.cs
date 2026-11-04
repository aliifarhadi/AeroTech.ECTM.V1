namespace AeroTech.ECTM.RestApi.V1.EngineInstallationAggregate.Requests
{
    public sealed record CloseInstallationRequest(
        DateTimeOffset RemovedUtc,
        string Reason);
}
