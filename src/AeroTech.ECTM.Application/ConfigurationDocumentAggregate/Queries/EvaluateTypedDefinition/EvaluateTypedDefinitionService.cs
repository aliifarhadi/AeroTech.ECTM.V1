using System.Text.Json;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition
{
    public sealed class EvaluateTypedDefinitionService : IEvaluateTypedDefinitionService
    {
        private const string DefinitionPath = "$.definition";

        private readonly IConfigurationDocumentRepository _documents;
        private readonly ConfigurationDocumentLoader _documentLoader;

        public EvaluateTypedDefinitionService(IConfigurationDocumentRepository documents, ConfigurationDocumentLoader documentLoader)
        {
            _documents = documents;
            _documentLoader = documentLoader;
        }

        public async Task<BackofficeTypedEvaluationDto> ExecuteAsync(IEvaluateTypedDefinitionQuery query, CancellationToken cancellationToken = default)
        {
            var catalogDocument = await CatalogDocumentAsync(query.CatalogDocumentId, cancellationToken);
            var catalog = ParameterCatalogValidator.Validate(catalogDocument.Content).Catalog
                          ?? throw ExceptionFactory.ConfigurationDocumentIsInvalid(catalogDocument.Id, 1);

            var entries = EntriesWithout(catalog, CodeOf(query.Definition));
            var validation = ParameterCatalogValidator.ValidateDefinition(query.Definition, entries, DefinitionPath, allowExistingCode: true);

            if (!validation.IsValid)
            {
                var firstIssue = validation.Issues[0];
                throw ExceptionFactory.TypedDefinitionIsInvalid($"{firstIssue.Path} {firstIssue.Code}: {firstIssue.Message}");
            }

            var inputs = query.Inputs.ToDictionary(input => input.Key, input => input.Value.ToArgs(), StringComparer.Ordinal);
            var evaluation = DerivedParameterEvaluator.Evaluate(validation.Definition!, entries, query.Position, inputs);

            return BackofficeTypedEvaluationDto.From(catalogDocument.Id, catalogDocument.VersionNumber, evaluation);
        }

        private async Task<ConfigurationDocument> CatalogDocumentAsync(long? catalogDocumentId, CancellationToken cancellationToken)
        {
            if (catalogDocumentId is { } documentId)
                return await _documentLoader.LoadAsync(nameof(ConfigurationKind.PARAMETER_CATALOG), documentId, cancellationToken);

            return await _documents.FindActiveAsync(ConfigurationKind.PARAMETER_CATALOG, cancellationToken)
                   ?? throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.PARAMETER_CATALOG);
        }

        private static string? CodeOf(JsonElement definition)
        {
            if (definition.ValueKind != JsonValueKind.Object)
                return null;

            if (!definition.TryGetProperty("code", out var code))
                return null;

            if (code.ValueKind != JsonValueKind.String)
                return null;

            return code.GetString();
        }

        private static Dictionary<string, CatalogEntry> EntriesWithout(ValidatedCatalog catalog, string? code)
            => catalog.Entries
                .Where(entry => entry.Key != code)
                .ToDictionary(entry => entry.Key, entry => entry.Value, StringComparer.Ordinal);
    }
}
