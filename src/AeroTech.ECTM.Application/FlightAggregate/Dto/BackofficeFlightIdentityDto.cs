using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeFlightIdentityDto(
        long? AircraftId,
        string? Registration,
        EnumValueDto? RegistrationSource,
        DateOnly? FlightDate,
        EnumValueDto? FlightDateSource,
        string? FlightNumber,
        EnumValueDto? FlightNumberSource,
        EnumValueDto IdentityStatus,
        IReadOnlyList<string> Reasons,
        IReadOnlyList<BackofficeEngineAssignmentDto> Engines)
    {
        public static BackofficeFlightIdentityDto From(IdentityResolution identity, IReadOnlyDictionary<long, string> serialByEngine)
            => new(
                identity.AircraftId,
                identity.Registration,
                EnumValueDto.OfNullable(identity.RegistrationSource),
                identity.FlightDate,
                EnumValueDto.OfNullable(identity.FlightDateSource),
                identity.FlightNumber,
                EnumValueDto.OfNullable(identity.FlightNumberSource),
                EnumValueDto.Of(identity.Status),
                identity.Reasons,
                identity.Engines
                    .OrderBy(engine => engine.Position)
                    .Select(engine => BackofficeEngineAssignmentDto.From(engine, serialByEngine))
                    .ToList());

        public static BackofficeFlightIdentityDto From(Flight flight, IReadOnlyDictionary<long, string> serialByEngine)
            => new(
                flight.AircraftId,
                flight.Registration,
                EnumValueDto.OfNullable(flight.RegistrationSource),
                flight.FlightDate,
                EnumValueDto.OfNullable(flight.FlightDateSource),
                flight.FlightNumber,
                EnumValueDto.OfNullable(flight.FlightNumberSource),
                EnumValueDto.Of(flight.IdentityStatus),
                flight.IdentityReasons,
                flight.Engines
                    .OrderBy(engine => engine.Position)
                    .Select(engine => BackofficeEngineAssignmentDto.From(engine, serialByEngine))
                    .ToList());
    }
}
