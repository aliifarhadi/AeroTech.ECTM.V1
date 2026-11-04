using System.Text.Json;
using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application._Shared.Json;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeConfigurationDocumentDetailDto(
        long Id,
        EnumValueDto Kind,
        int VersionNumber,
        EnumValueDto Status,
        long? BasedOnDocumentId,
        string ContentSha256,
        DateTimeOffset LastUpdateTime,
        JsonElement Content)
    {
        public static BackofficeConfigurationDocumentDetailDto From(ConfigurationDocument document)
            => new(
                document.Id,
                EnumValueDto.Of(document.Kind),
                document.VersionNumber,
                EnumValueDto.Of(document.Status),
                document.BasedOnDocumentId,
                document.ContentSha256,
                document.LastUpdateTime,
                StoredJson.Parse(document.Content));
    }
}
