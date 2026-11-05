using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeAnchorChangeDto(string Code, int? BeforeIndex, int? AfterIndex, EnumValueDto? BeforeBasis, EnumValueDto? AfterBasis)
    {
        public static BackofficeAnchorChangeDto From(AnchorChange change)
            => new(change.Code, change.BeforeIndex, change.AfterIndex, EnumValueDto.OfNullable(change.BeforeBasis), EnumValueDto.OfNullable(change.AfterBasis));
    }
}
