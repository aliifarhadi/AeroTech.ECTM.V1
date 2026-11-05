using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Dto
{
    public sealed record BackofficeFlightQualityRowDto(
        long FlightId,
        DateOnly? FlightDate,
        string? FlightNumber,
        EnumValueDto? Grade,
        EnumValueDto? TimeTrust,
        int? DiscontinuityCount,
        bool? PartialStart,
        bool? PartialEnd);
}
