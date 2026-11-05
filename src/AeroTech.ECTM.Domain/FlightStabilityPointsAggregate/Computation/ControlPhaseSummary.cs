using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record ControlPhaseSummary(
        FlightPhaseCode Phase,
        int Samples,
        int? StartIndex,
        int? EndIndexInclusive,
        IReadOnlyList<ParameterSummary> EngineChannels,
        IReadOnlyList<ModeObservation> Modes);
}
