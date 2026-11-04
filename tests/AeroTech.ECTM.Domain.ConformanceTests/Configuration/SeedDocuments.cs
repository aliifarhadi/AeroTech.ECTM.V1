using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConformanceTests.Configuration;

public static class SeedDocuments
{
    public static string Catalog => ConfigurationSeeds.ContentOf(ConfigurationKind.PARAMETER_CATALOG);

    public static string Profile => ConfigurationSeeds.ContentOf(ConfigurationKind.DATA_PROFILE);

    public static ValidatedCatalog ValidatedCatalog => ParameterCatalogValidator.Validate(Catalog).Catalog!;

    public static string Changed(string content, Action<JsonObject> change)
    {
        var document = JsonNode.Parse(content)!.AsObject();
        change(document);

        return document.ToJsonString();
    }

    public static JsonObject ColumnAt(JsonObject profile, int ordinal)
        => profile["columns"]!.AsArray().Single(column => column!["ordinal"]!.GetValue<int>() == ordinal)!.AsObject();
}
