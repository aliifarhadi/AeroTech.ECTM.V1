using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Dto
{
    public sealed record BackofficeGradeCountDto(EnumValueDto Grade, int Flights);
}
