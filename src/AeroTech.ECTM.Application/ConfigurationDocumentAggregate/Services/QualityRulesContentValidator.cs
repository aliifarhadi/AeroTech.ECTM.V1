using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class QualityRulesContentValidator : IConfigurationContentValidator
    {
        private readonly ActiveCatalogLoader _activeCatalog;

        public QualityRulesContentValidator(ActiveCatalogLoader activeCatalog) => _activeCatalog = activeCatalog;

        public ConfigurationKind Kind => ConfigurationKind.QUALITY_RULES;

        public async Task<IReadOnlyList<ValidationIssue>> ValidateAsync(string content, bool forPublish, CancellationToken cancellationToken = default)
        {
            var catalog = await _activeCatalog.LoadAsync(cancellationToken);
            var validation = QualityRulesValidator.Validate(content, catalog);

            if (forPublish && catalog is null)
                return [.. validation.Issues, new ValidationIssue("$", QualityRulesValidator.ActiveCatalogRequired, "Publishing QUALITY_RULES needs an active PARAMETER_CATALOG to check its channels.")];

            return validation.Issues;
        }
    }
}
