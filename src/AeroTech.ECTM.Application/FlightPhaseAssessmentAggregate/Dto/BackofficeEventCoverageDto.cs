using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeEventCoverageDto(
        string Code,
        string ParameterKey,
        int? Position,
        EnumValueDto Evaluability,
        int ValuedSamples,
        int ActiveSamples,
        int SpanCount,
        string? Note)
    {
        public static BackofficeEventCoverageDto From(EventChannelCoverage coverage)
            => new(
                coverage.Code,
                coverage.ParameterKey,
                coverage.Position,
                EnumValueDto.Of(coverage.Evaluability),
                coverage.ValuedSamples,
                coverage.ActiveSamples,
                coverage.SpanCount,
                coverage.Note);
    }
}
