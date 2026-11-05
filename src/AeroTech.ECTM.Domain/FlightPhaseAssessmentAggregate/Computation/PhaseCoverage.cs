using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseCoverage(FlightPhaseCode PhaseCode, int Samples, double Percent);
}
