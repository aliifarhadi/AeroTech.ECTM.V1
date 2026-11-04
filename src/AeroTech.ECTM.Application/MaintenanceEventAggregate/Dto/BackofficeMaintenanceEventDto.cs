using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto
{
    public sealed record BackofficeMaintenanceEventDto(
        long Id,
        EnumValueDto IdentityKind,
        long? EngineId,
        string? EngineSerialNumber,
        long? AircraftId,
        string? AircraftRegistration,
        int? Position,
        EnumValueDto EventType,
        DateTimeOffset EventUtc,
        EnumValueDto DateReliability,
        decimal? HoursAtEvent,
        int? CyclesAtEvent,
        string? WorkOrderReference,
        string? Notes)
    {
        public static BackofficeMaintenanceEventDto From(MaintenanceEvent maintenanceEvent, string? engineSerialNumber, string? aircraftRegistration)
            => new(
                maintenanceEvent.Id,
                EnumValueDto.Of(maintenanceEvent.IdentityKind),
                maintenanceEvent.EngineId,
                engineSerialNumber,
                maintenanceEvent.AircraftId,
                aircraftRegistration,
                maintenanceEvent.Position,
                EnumValueDto.Of(maintenanceEvent.EventType),
                maintenanceEvent.EventAt,
                EnumValueDto.Of(maintenanceEvent.DateReliability),
                maintenanceEvent.HoursAtEvent,
                maintenanceEvent.CyclesAtEvent,
                maintenanceEvent.WorkOrderReference,
                maintenanceEvent.Notes);
    }
}
