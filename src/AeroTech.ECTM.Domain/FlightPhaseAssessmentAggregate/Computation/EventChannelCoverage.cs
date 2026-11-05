using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record EventChannelCoverage(
        string Code,
        string ParameterKey,
        int? Position,
        EventEvaluability Evaluability,
        int ValuedSamples,
        int ActiveSamples,
        int SpanCount,
        string? Note);
}
