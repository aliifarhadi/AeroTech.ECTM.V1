using System.Text.Json;
using AeroTech.ECTM.Application._Shared.Json;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeJsonChangeDto(string Path, string Change, JsonElement? Before, JsonElement? After)
    {
        public static BackofficeJsonChangeDto From(JsonChange change)
            => new(change.Path, change.Change, StoredJson.ParseNullable(change.Before), StoredJson.ParseNullable(change.After));
    }
}
