using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent
{
    public interface IUpdateMaintenanceEventCommand
    {
        long MaintenanceEventId { get; }

        MaintenanceEventType EventType { get; }

        DateTimeOffset EventUtc { get; }

        DateReliability DateReliability { get; }

        decimal? HoursAtEvent { get; }

        int? CyclesAtEvent { get; }

        string? WorkOrderReference { get; }

        string? Notes { get; }
    }
}
