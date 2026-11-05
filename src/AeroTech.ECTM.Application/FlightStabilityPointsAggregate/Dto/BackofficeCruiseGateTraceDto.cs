using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeCruiseGateTraceDto(
        int EligibleWindowEnds,
        IReadOnlyList<BackofficeGateCountDto> RejectedWindowEnds,
        IReadOnlyList<BackofficeGateValueDto> ReportWindowGates,
        IReadOnlyList<BackofficeRejectedWindowDto> NearestRejected)
    {
        public static BackofficeCruiseGateTraceDto From(CruiseGateTrace trace)
            => new(
                trace.EligibleWindowEnds,
                trace.RejectedWindowEnds.Select(pair => new BackofficeGateCountDto(EnumValueDto.Of(pair.Key), pair.Value)).ToList(),
                trace.ReportWindowGates.Select(gate => new BackofficeGateValueDto(EnumValueDto.Of(gate.Gate), gate.Actual, gate.Limit, gate.Passes)).ToList(),
                trace.NearestRejected.Select(window => new BackofficeRejectedWindowDto(window.WindowEndIndex, EnumValueDto.Of(window.FailedGate))).ToList());
    }

    public sealed record BackofficeGateCountDto(EnumValueDto Gate, int WindowEnds);

    public sealed record BackofficeGateValueDto(EnumValueDto Gate, double Actual, double Limit, bool Passes);

    public sealed record BackofficeRejectedWindowDto(int WindowEndIndex, EnumValueDto FailedGate);
}
