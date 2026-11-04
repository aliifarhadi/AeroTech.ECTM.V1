using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.RestApi.V1.AircraftAggregate.Requests
{
    public sealed record UpdateAircraftRequest(
        string? TypeCode,
        string? OperatorCode,
        AircraftStatus Status);
}
