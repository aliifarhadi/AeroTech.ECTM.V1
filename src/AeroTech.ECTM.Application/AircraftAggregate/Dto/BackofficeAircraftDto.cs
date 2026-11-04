using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.AircraftAggregate;

namespace AeroTech.ECTM.Application.AircraftAggregate.Dto
{
    public sealed record BackofficeAircraftDto(
        long Id,
        string Registration,
        string? TypeCode,
        string? OperatorCode,
        int EngineCount,
        EnumValueDto Status,
        IReadOnlyList<BackofficeEngineInstallationDto> CurrentInstallations)
    {
        public static BackofficeAircraftDto From(Aircraft aircraft, IEnumerable<BackofficeEngineInstallationDto> openInstallations)
            => new(
                aircraft.Id,
                aircraft.Registration,
                aircraft.TypeCode,
                aircraft.OperatorCode,
                aircraft.EngineCount,
                EnumValueDto.Of(aircraft.Status),
                openInstallations
                    .Where(installation => installation.AircraftId == aircraft.Id)
                    .OrderBy(installation => installation.Position)
                    .ToList());
    }
}
