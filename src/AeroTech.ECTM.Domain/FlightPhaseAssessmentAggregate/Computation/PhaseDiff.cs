namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseDiff(
        int SampleCount,
        int ChangedSamples,
        double ChangedPercent,
        IReadOnlyList<PhaseChange> Changes,
        IReadOnlyList<AnchorChange> Anchors);
}
