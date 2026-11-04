using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeReadinessKindDto(
        EnumValueDto Kind,
        string DeliveryPhase,
        bool RequiredForParser,
        long? ActiveDocumentId,
        int? ActiveVersionNumber);
}
