using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Entities
{
    public sealed class FlightEngineAssignment : Entity<long>
    {
        private FlightEngineAssignment()
        {
        }

        internal FlightEngineAssignment(long id, long flightId, EngineResolution resolution)
        {
            Id = id;
            FlightId = flightId;
            Position = (byte)resolution.Position;
            Assign(resolution);
        }

        public long FlightId { get; private set; }

        public byte Position { get; private set; }

        public EngineAssignmentStatus Status { get; private set; }

        public long? EngineId { get; private set; }

        public long? InstallationId { get; private set; }

        internal void Assign(EngineResolution resolution)
        {
            Status = resolution.Status;
            EngineId = resolution.EngineId;
            InstallationId = resolution.InstallationId;
        }
    }
}
