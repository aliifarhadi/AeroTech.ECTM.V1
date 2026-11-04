using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent.Backoffice
{
    public sealed record BackofficeCreateMaintenanceEventCommand(
        long? EngineId,
        long? AircraftId,
        int? Position,
        MaintenanceEventType EventType,
        DateTimeOffset EventUtc,
        DateReliability DateReliability,
        decimal? HoursAtEvent,
        int? CyclesAtEvent,
        string? WorkOrderReference,
        string? Notes) : IRequest<BackofficeMaintenanceEventDto>, ICreateMaintenanceEventCommand;
}
