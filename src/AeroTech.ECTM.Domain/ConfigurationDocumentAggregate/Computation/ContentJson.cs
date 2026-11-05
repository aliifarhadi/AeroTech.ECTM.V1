using System.Text.Json;
using System.Text.Json.Serialization;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class ContentJson
    {
        public const string ShapeInvalid = "CONTENT_SHAPE_INVALID";

        private static readonly JsonSerializerOptions StrictOptions = new(JsonSerializerDefaults.Web)
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            AllowOutOfOrderMetadataProperties = true,
            Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
        };

        public static TContent? Read<TContent>(string content, List<ValidationIssue> issues)
            where TContent : class
        {
            try
            {
                return JsonSerializer.Deserialize<TContent>(content, StrictOptions);
            }
            catch (JsonException exception)
            {
                issues.Add(new ValidationIssue(exception.Path ?? "$", ShapeInvalid, exception.Message));
                return null;
            }
            catch (NotSupportedException exception)
            {
                issues.Add(new ValidationIssue("$", ShapeInvalid, exception.Message));
                return null;
            }
        }

        public static TContent? Read<TContent>(JsonElement content, List<ValidationIssue> issues)
            where TContent : class
            => Read<TContent>(content.GetRawText(), issues);
    }
}
