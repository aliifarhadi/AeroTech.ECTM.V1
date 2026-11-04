using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.RestApi.V1.MaintenanceEventAggregate.Requests
{
    public sealed record UpdateMaintenanceEventRequest(
        MaintenanceEventType EventType,
        DateTimeOffset EventUtc,
        DateReliability DateReliability,
        decimal? HoursAtEvent,
        int? CyclesAtEvent,
        string? WorkOrderReference,
        string? Notes);
}
