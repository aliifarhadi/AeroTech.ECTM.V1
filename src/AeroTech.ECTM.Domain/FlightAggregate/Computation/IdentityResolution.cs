using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record IdentityResolution(
        long? AircraftId,
        string? Registration,
        IdentitySource? RegistrationSource,
        DateOnly? FlightDate,
        IdentitySource? FlightDateSource,
        string? FlightNumber,
        IdentitySource? FlightNumberSource,
        IdentityStatus Status,
        IReadOnlyList<string> Reasons,
        IReadOnlyList<EngineResolution> Engines);
}
