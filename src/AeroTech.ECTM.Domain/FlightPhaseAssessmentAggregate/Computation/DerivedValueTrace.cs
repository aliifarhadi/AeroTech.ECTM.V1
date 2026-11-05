namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record DerivedValueTrace(
        string Code,
        string Operator,
        double? Value,
        IReadOnlyList<int> SourceIndices,
        string? NullReason);
}
