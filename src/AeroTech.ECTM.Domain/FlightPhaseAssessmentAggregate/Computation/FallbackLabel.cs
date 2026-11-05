using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record FallbackLabel(FlightPhaseCode? Label, PhaseUnknownReason? UnknownReason);
}
