namespace AeroTech.ECTM.RestApi.V1.AircraftAggregate.Requests
{
    public sealed record CreateAircraftRequest(
        string Registration,
        string? TypeCode,
        string? OperatorCode);
}
