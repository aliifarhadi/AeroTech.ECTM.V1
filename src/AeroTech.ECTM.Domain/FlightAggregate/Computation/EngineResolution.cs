using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record EngineResolution(int Position, EngineAssignmentStatus Status, long? EngineId, long? InstallationId);
}
