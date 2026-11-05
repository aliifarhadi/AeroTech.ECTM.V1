using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeAnchorDto(
        string Code,
        string Operator,
        int? Index,
        double? Elapsed,
        EnumValueDto Basis,
        string Channel,
        double? NumberBefore,
        double? NumberAt,
        string? TextBefore,
        string? TextAt,
        BackofficePredicateTraceDto? At)
    {
        public static BackofficeAnchorDto From(AnchorResult anchor)
            => new(
                anchor.Code,
                anchor.Operator,
                anchor.Index,
                anchor.Elapsed,
                EnumValueDto.Of(anchor.Basis),
                anchor.Channel,
                anchor.NumberBefore,
                anchor.NumberAt,
                anchor.TextBefore,
                anchor.TextAt,
                BackofficePredicateTraceDto.FromNullable(anchor.At));
    }
}
