using System.Text.Json;

namespace AeroTech.ECTM.Application._Shared.Json
{
    public static class StoredJson
    {
        public static JsonElement Parse(string json)
        {
            using var document = JsonDocument.Parse(json);

            return document.RootElement.Clone();
        }

        public static JsonElement? ParseNullable(string? json)
            => json is null ? null : Parse(json);
    }
}
