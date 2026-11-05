namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed record QualitySpan(
        string Code,
        int StartIndex,
        int EndIndexInclusive,
        IReadOnlyList<string> ParameterKeys,
        IReadOnlyList<int> ProfileOrdinals,
        int? JumpIndex,
        string? Cause,
        double? Actual,
        double? Threshold,
        string? Unit,
        IReadOnlyList<string> Affects,
        string? Note);
}
