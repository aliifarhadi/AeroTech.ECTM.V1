using System.Text.Json;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition
{
    public interface IEvaluateTypedDefinitionQuery
    {
        long? CatalogDocumentId { get; }

        JsonElement Definition { get; }

        int? Position { get; }

        IReadOnlyDictionary<string, EvaluationInput> Inputs { get; }
    }
}
