using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services
{
    public sealed class ComparisonTolerancesLoader
    {
        private readonly IConfigurationDocumentRepository _documents;

        public ComparisonTolerancesLoader(IConfigurationDocumentRepository documents) => _documents = documents;

        public async Task<ComparisonTolerances> LoadActiveAsync(CancellationToken cancellationToken = default)
        {
            var document = await _documents.FindActiveAsync(ConfigurationKind.BASELINE_RULES, cancellationToken);

            if (document is null)
                return ComparisonTolerances.None;

            var validation = BaselineRulesValidator.Validate(document.Content, null);

            if (!validation.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(document.Id, validation.Issues.Count);

            return new ComparisonTolerances(document.Id, validation.Rules!.Conditions ?? []);
        }
    }
}
