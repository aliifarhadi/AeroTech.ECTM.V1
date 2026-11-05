using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseChange(int StartIndex, int EndIndexInclusive, FlightPhaseCode? Before, FlightPhaseCode? After);
}
