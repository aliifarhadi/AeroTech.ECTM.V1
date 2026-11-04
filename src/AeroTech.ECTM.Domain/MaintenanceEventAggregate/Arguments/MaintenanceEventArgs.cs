using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.MaintenanceEventAggregate.Arguments
{
    public sealed record MaintenanceEventArgs(
        MaintenanceEventType EventType,
        DateTimeOffset EventAt,
        DateReliability DateReliability,
        decimal? HoursAtEvent,
        int? CyclesAtEvent,
        string? WorkOrderReference,
        string? Notes);
}
