using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeRuleDocumentHeaderDto(
        string Code,
        string Title,
        string Description,
        EnumValueDto CalibrationStatus,
        EnumValueDto EngineeringSignOff)
    {
        public static BackofficeRuleDocumentHeaderDto From(RuleDocumentHeader header)
            => new(header.Code, header.Title, header.Description, EnumValueDto.Of(header.CalibrationStatus), EnumValueDto.Of(header.EngineeringSignOff));
    }
}
