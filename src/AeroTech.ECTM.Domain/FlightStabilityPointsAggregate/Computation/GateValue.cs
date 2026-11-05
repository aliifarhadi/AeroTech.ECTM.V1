using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record GateValue(CruiseGate Gate, double Actual, double Limit, bool Passes);
}
