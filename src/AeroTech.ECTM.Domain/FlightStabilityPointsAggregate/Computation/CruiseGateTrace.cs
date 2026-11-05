using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record CruiseGateTrace(
        int EligibleWindowEnds,
        IReadOnlyDictionary<CruiseGate, int> RejectedWindowEnds,
        IReadOnlyList<GateValue> ReportWindowGates,
        IReadOnlyList<RejectedWindow> NearestRejected);
}
