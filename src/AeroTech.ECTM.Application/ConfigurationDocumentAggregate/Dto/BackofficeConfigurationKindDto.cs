using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeConfigurationKindDto(
        EnumValueDto Kind,
        string DeliveryPhase,
        bool Authorable,
        BackofficeConfigurationDocumentDto? ActiveVersion,
        string? ActiveVersionReason,
        int DraftCount,
        int PublishedCount)
    {
        public const string NoActiveVersion = "NO_ACTIVE_VERSION";

        public static BackofficeConfigurationKindDto From(
            ConfigurationKind kind,
            string deliveryPhase,
            bool authorable,
            ConfigurationDocument? activeDocument,
            int draftCount,
            int publishedCount)
        {
            if (activeDocument is null)
                return new(EnumValueDto.Of(kind), deliveryPhase, authorable, null, NoActiveVersion, draftCount, publishedCount);

            var activeVersion = BackofficeConfigurationDocumentDto.From(activeDocument);

            return new(EnumValueDto.Of(kind), deliveryPhase, authorable, activeVersion, null, draftCount, publishedCount);
        }
    }
}
