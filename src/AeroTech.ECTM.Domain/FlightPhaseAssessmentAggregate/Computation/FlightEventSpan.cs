using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record FlightEventSpan(
        string Code,
        string ParameterKey,
        int? Position,
        int StartIndex,
        int EndIndexInclusive,
        double StartElapsed,
        double EndElapsed,
        double Seconds,
        FlightPhaseCode PhaseAtStart,
        FlightPhaseCode PhaseAtEnd,
        IReadOnlyList<EventValue> Values);
}
