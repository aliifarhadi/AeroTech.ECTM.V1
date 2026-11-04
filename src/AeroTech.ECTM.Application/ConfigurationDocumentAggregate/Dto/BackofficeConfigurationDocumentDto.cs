using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeConfigurationDocumentDto(
        long Id,
        EnumValueDto Kind,
        int VersionNumber,
        EnumValueDto Status,
        long? BasedOnDocumentId,
        string ContentSha256,
        DateTimeOffset LastUpdateTime,
        DateTimeOffset? PublishedUtc,
        string? PublishedBy,
        string? ApprovalNote,
        DateTimeOffset? RetiredUtc,
        string? RetiredBy,
        string? RetirementNote)
    {
        public static BackofficeConfigurationDocumentDto From(ConfigurationDocument document)
            => new(
                document.Id,
                EnumValueDto.Of(document.Kind),
                document.VersionNumber,
                EnumValueDto.Of(document.Status),
                document.BasedOnDocumentId,
                document.ContentSha256,
                document.LastUpdateTime,
                document.PublishedAt,
                document.PublishedBy,
                document.ApprovalNote,
                document.RetiredAt,
                document.RetiredBy,
                document.RetirementNote);
    }
}
