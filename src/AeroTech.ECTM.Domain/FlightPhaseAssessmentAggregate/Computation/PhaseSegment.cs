using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseSegment(
        FlightPhaseCode PhaseCode,
        int StartIndex,
        int EndIndexInclusive,
        double StartElapsed,
        double EndElapsed,
        double Seconds,
        PhaseConfidence Confidence,
        IReadOnlyList<string> EvidenceCodes,
        IReadOnlyList<string> Flags,
        IReadOnlyList<PhaseUnknownReason> UnknownReasons);
}
