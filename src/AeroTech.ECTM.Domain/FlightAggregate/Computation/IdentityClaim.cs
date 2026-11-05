using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record IdentityClaim(
        string? Registration,
        IdentitySource? RegistrationSource,
        DateOnly? FlightDate,
        IdentitySource? FlightDateSource,
        string? FlightNumber,
        IdentitySource? FlightNumberSource,
        IReadOnlyList<string> Reasons);
}
