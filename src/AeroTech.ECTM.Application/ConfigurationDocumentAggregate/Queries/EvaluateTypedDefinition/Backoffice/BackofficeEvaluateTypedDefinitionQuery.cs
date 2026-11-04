using System.Text.Json;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition.Backoffice
{
    public sealed record BackofficeEvaluateTypedDefinitionQuery(
        long? CatalogDocumentId,
        JsonElement Definition,
        int? Position,
        IReadOnlyDictionary<string, EvaluationInput> Inputs) : IRequest<BackofficeTypedEvaluationDto>, IEvaluateTypedDefinitionQuery;
}
