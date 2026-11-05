using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class SnapshotRulesContentValidator : IConfigurationContentValidator
    {
        private readonly ActiveCatalogLoader _activeCatalog;

        public SnapshotRulesContentValidator(ActiveCatalogLoader activeCatalog) => _activeCatalog = activeCatalog;

        public ConfigurationKind Kind => ConfigurationKind.SNAPSHOT_RULES;

        public async Task<IReadOnlyList<ValidationIssue>> ValidateAsync(string content, bool forPublish, CancellationToken cancellationToken = default)
        {
            var catalog = await _activeCatalog.LoadAsync(cancellationToken);
            var validation = SnapshotRulesValidator.Validate(content, catalog);

            if (forPublish && catalog is null)
                return [.. validation.Issues, new ValidationIssue("$", SnapshotRulesValidator.ActiveCatalogRequired, "Publishing SNAPSHOT_RULES needs an active PARAMETER_CATALOG to check its channels.")];

            return validation.Issues;
        }
    }
}
