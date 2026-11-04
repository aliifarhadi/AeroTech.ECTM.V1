using System.Text.Json;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class JsonDiff
    {
        public const string Added = "ADDED";
        public const string Removed = "REMOVED";
        public const string Changed = "CHANGED";

        public static IReadOnlyList<JsonChange> Compare(string before, string after)
        {
            using var beforeDocument = JsonDocument.Parse(before);
            using var afterDocument = JsonDocument.Parse(after);

            var changes = new List<JsonChange>();
            CompareElements(beforeDocument.RootElement, afterDocument.RootElement, "$", changes);

            return changes;
        }

        private static void CompareElements(JsonElement before, JsonElement after, string path, List<JsonChange> changes)
        {
            if (JsonElement.DeepEquals(before, after))
                return;

            var bothObjects = before.ValueKind == JsonValueKind.Object && after.ValueKind == JsonValueKind.Object;
            var bothArrays = before.ValueKind == JsonValueKind.Array && after.ValueKind == JsonValueKind.Array;

            if (bothObjects)
            {
                CompareObjects(before, after, path, changes);
                return;
            }

            if (bothArrays)
            {
                CompareArrays(before, after, path, changes);
                return;
            }

            changes.Add(new JsonChange(path, Changed, before.GetRawText(), after.GetRawText()));
        }

        private static void CompareObjects(JsonElement before, JsonElement after, string path, List<JsonChange> changes)
        {
            foreach (var property in before.EnumerateObject())
            {
                var propertyPath = $"{path}.{property.Name}";

                if (after.TryGetProperty(property.Name, out var afterValue))
                    CompareElements(property.Value, afterValue, propertyPath, changes);
                else
                    changes.Add(new JsonChange(propertyPath, Removed, property.Value.GetRawText(), null));
            }

            foreach (var property in after.EnumerateObject())
            {
                if (!before.TryGetProperty(property.Name, out _))
                    changes.Add(new JsonChange($"{path}.{property.Name}", Added, null, property.Value.GetRawText()));
            }
        }

        private static void CompareArrays(JsonElement before, JsonElement after, string path, List<JsonChange> changes)
        {
            var beforeItems = before.EnumerateArray().ToList();
            var afterItems = after.EnumerateArray().ToList();
            var shared = Math.Min(beforeItems.Count, afterItems.Count);

            for (var index = 0; index < shared; index++)
                CompareElements(beforeItems[index], afterItems[index], $"{path}[{index}]", changes);

            for (var index = shared; index < beforeItems.Count; index++)
                changes.Add(new JsonChange($"{path}[{index}]", Removed, beforeItems[index].GetRawText(), null));

            for (var index = shared; index < afterItems.Count; index++)
                changes.Add(new JsonChange($"{path}[{index}]", Added, null, afterItems[index].GetRawText()));
        }
    }
}
