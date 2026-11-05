namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record MaskTrace(string Code, int StartIndex, int EndIndexInclusive, int? JumpIndex, string? Cause);
}
