using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class BaselineRulesContentValidator : IConfigurationContentValidator
    {
        private readonly ActiveCatalogLoader _activeCatalog;

        public BaselineRulesContentValidator(ActiveCatalogLoader activeCatalog) => _activeCatalog = activeCatalog;

        public ConfigurationKind Kind => ConfigurationKind.BASELINE_RULES;

        public async Task<IReadOnlyList<ValidationIssue>> ValidateAsync(string content, bool forPublish, CancellationToken cancellationToken = default)
        {
            var catalog = await _activeCatalog.LoadAsync(cancellationToken);
            var validation = BaselineRulesValidator.Validate(content, catalog);

            if (forPublish && catalog is null)
                return [.. validation.Issues, new ValidationIssue("$", BaselineRulesValidator.ActiveCatalogRequired, "Publishing BASELINE_RULES needs an active PARAMETER_CATALOG to check its channels.")];

            return validation.Issues;
        }
    }
}
