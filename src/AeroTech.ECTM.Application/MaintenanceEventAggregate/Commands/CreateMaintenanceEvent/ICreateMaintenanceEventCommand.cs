using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent
{
    public interface ICreateMaintenanceEventCommand
    {
        long? EngineId { get; }

        long? AircraftId { get; }

        int? Position { get; }

        MaintenanceEventType EventType { get; }

        DateTimeOffset EventUtc { get; }

        DateReliability DateReliability { get; }

        decimal? HoursAtEvent { get; }

        int? CyclesAtEvent { get; }

        string? WorkOrderReference { get; }

        string? Notes { get; }
    }
}
