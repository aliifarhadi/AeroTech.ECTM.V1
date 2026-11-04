using System.Text.Json;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ParameterCatalogContent(
        string? CatalogCode,
        string? EngineFamily,
        string? Source,
        IReadOnlyList<CatalogParameter>? Parameters,
        IReadOnlyList<JsonElement>? DerivedParameters);
}
