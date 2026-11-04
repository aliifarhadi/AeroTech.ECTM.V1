using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.MaintenanceEventAggregate
{
    public sealed class MaintenanceEvent : AggregateRoot<long>
    {
        private MaintenanceEvent()
        {
        }

        private MaintenanceEvent(long id, IdentityKind identityKind, long? engineId, long? aircraftId, byte? position)
        {
            Id = id;
            IdentityKind = identityKind;
            EngineId = engineId;
            AircraftId = aircraftId;
            Position = position;
        }

        public IdentityKind IdentityKind { get; private set; }

        public long? EngineId { get; private set; }

        public long? AircraftId { get; private set; }

        public byte? Position { get; private set; }

        public MaintenanceEventType EventType { get; private set; }

        public DateTimeOffset EventAt { get; private set; }

        public DateReliability DateReliability { get; private set; }

        public decimal? HoursAtEvent { get; private set; }

        public int? CyclesAtEvent { get; private set; }

        public string? WorkOrderReference { get; private set; }

        public string? Notes { get; private set; }

        public static MaintenanceEvent ForEngine(long id, long engineId, MaintenanceEventArgs args)
        {
            var maintenanceEvent = new MaintenanceEvent(id, IdentityKind.ENGINE, engineId, null, null);
            maintenanceEvent.Update(args);

            return maintenanceEvent;
        }

        public static MaintenanceEvent ForPosition(long id, long aircraftId, int position, MaintenanceEventArgs args)
        {
            var validPosition = EnginePositions.EnsureValid(position);

            var maintenanceEvent = new MaintenanceEvent(id, IdentityKind.POSITION_ONLY, null, aircraftId, validPosition);
            maintenanceEvent.Update(args);

            return maintenanceEvent;
        }

        public void Update(MaintenanceEventArgs args)
        {
            if (args.HoursAtEvent < 0)
                throw ExceptionFactory.MaintenanceEventCountersAreInvalid();

            if (args.CyclesAtEvent < 0)
                throw ExceptionFactory.MaintenanceEventCountersAreInvalid();

            EventType = args.EventType;
            EventAt = args.EventAt.ToUniversalTime();
            DateReliability = args.DateReliability;
            HoursAtEvent = args.HoursAtEvent;
            CyclesAtEvent = args.CyclesAtEvent;
            WorkOrderReference = OptionalText.Normalize(args.WorkOrderReference);
            Notes = OptionalText.Normalize(args.Notes);
        }
    }
}
