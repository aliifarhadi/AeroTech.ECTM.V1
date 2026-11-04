using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeConfigurationKindDto(
        EnumValueDto Kind,
        BackofficeConfigurationDocumentDto? ActiveVersion,
        string? ActiveVersionReason,
        int DraftCount,
        int PublishedCount)
    {
        public const string NoActiveVersion = "NO_ACTIVE_VERSION";

        public static BackofficeConfigurationKindDto From(ConfigurationKind kind, ConfigurationDocument? activeDocument, int draftCount, int publishedCount)
        {
            if (activeDocument is null)
                return new(EnumValueDto.Of(kind), null, NoActiveVersion, draftCount, publishedCount);

            return new(EnumValueDto.Of(kind), BackofficeConfigurationDocumentDto.From(activeDocument), null, draftCount, publishedCount);
        }
    }
}
