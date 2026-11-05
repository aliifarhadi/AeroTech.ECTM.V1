using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class PhaseRuleSetContentValidator : IConfigurationContentValidator
    {
        private readonly ActiveCatalogLoader _activeCatalog;

        public PhaseRuleSetContentValidator(ActiveCatalogLoader activeCatalog) => _activeCatalog = activeCatalog;

        public ConfigurationKind Kind => ConfigurationKind.PHASE_RULESET;

        public async Task<IReadOnlyList<ValidationIssue>> ValidateAsync(string content, bool forPublish, CancellationToken cancellationToken = default)
        {
            var catalog = await _activeCatalog.LoadAsync(cancellationToken);
            var validation = PhaseRuleSetValidator.Validate(content, catalog);

            if (forPublish && catalog is null)
                return [.. validation.Issues, new ValidationIssue("$", PhaseRuleSetValidator.ActiveCatalogRequired, "Publishing PHASE_RULESET needs an active PARAMETER_CATALOG to check its channels.")];

            return validation.Issues;
        }
    }
}
