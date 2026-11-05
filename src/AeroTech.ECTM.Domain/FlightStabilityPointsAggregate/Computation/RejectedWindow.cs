using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record RejectedWindow(int WindowEndIndex, CruiseGate FailedGate);
}
