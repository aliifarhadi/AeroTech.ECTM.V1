using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Arguments
{
    public sealed record FlightFilterArgs(
        FlightState? State,
        long? AircraftId,
        long? EngineId,
        DateOnly? From,
        DateOnly? To,
        long? ImportBatchId,
        IdentityStatus? IdentityStatus);
}
