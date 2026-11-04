using System.Text.Json;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition;

namespace AeroTech.ECTM.RestApi.V1.ConfigurationDocumentAggregate.Requests
{
    public sealed record EvaluateTypedDefinitionRequest(
        long? CatalogDocumentId,
        JsonElement Definition,
        int? Position,
        IReadOnlyDictionary<string, EvaluationInput> Inputs);
}
