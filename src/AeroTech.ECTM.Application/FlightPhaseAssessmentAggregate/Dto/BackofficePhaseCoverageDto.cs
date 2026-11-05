using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePhaseCoverageDto(EnumValueDto Phase, int Samples, double Percent)
    {
        public static BackofficePhaseCoverageDto From(PhaseCoverage coverage)
            => new(EnumValueDto.Of(coverage.PhaseCode), coverage.Samples, coverage.Percent);
    }
}
