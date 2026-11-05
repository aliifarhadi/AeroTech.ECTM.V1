using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeEventSpanDto(
        string Code,
        string ParameterKey,
        int? Position,
        int StartIndex,
        int EndIndexInclusive,
        double StartElapsed,
        double EndElapsed,
        double Seconds,
        EnumValueDto PhaseAtStart,
        EnumValueDto PhaseAtEnd,
        IReadOnlyList<EventValue> Values)
    {
        public static BackofficeEventSpanDto From(FlightEventSpan span)
            => new(
                span.Code,
                span.ParameterKey,
                span.Position,
                span.StartIndex,
                span.EndIndexInclusive,
                span.StartElapsed,
                span.EndElapsed,
                span.Seconds,
                EnumValueDto.Of(span.PhaseAtStart),
                EnumValueDto.Of(span.PhaseAtEnd),
                span.Values);
    }
}
