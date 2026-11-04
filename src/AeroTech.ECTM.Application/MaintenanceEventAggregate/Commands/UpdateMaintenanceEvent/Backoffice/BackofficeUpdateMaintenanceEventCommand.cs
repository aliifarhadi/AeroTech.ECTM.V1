using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent.Backoffice
{
    public sealed record BackofficeUpdateMaintenanceEventCommand(
        long MaintenanceEventId,
        MaintenanceEventType EventType,
        DateTimeOffset EventUtc,
        DateReliability DateReliability,
        decimal? HoursAtEvent,
        int? CyclesAtEvent,
        string? WorkOrderReference,
        string? Notes) : IRequest<BackofficeMaintenanceEventDto>, IUpdateMaintenanceEventCommand;
}
