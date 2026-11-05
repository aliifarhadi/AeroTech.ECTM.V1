namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record ControlContextResult(string VerificationNote, IReadOnlyList<ControlPhaseSummary> Phases);
}
