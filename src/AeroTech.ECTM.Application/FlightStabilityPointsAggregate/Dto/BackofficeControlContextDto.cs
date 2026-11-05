using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeControlContextDto(
        long FlightId,
        long AnalysisRevisionId,
        string VerificationNote,
        IReadOnlyList<BackofficeControlPhaseDto> Phases);

    public sealed record BackofficeControlPhaseDto(
        EnumValueDto Phase,
        int Samples,
        int? StartIndex,
        int? EndIndexInclusive,
        IReadOnlyList<ParameterSummary> EngineChannels,
        IReadOnlyList<ModeObservation> Modes)
    {
        public static BackofficeControlPhaseDto From(ControlPhaseSummary phase)
            => new(EnumValueDto.Of(phase.Phase), phase.Samples, phase.StartIndex, phase.EndIndexInclusive, phase.EngineChannels, phase.Modes);
    }
}
