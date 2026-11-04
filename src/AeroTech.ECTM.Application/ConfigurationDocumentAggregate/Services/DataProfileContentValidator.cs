using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class DataProfileContentValidator : IConfigurationContentValidator
    {
        private readonly ActiveCatalogLoader _activeCatalog;

        public DataProfileContentValidator(ActiveCatalogLoader activeCatalog) => _activeCatalog = activeCatalog;

        public ConfigurationKind Kind => ConfigurationKind.DATA_PROFILE;

        public async Task<IReadOnlyList<ValidationIssue>> ValidateAsync(string content, bool forPublish, CancellationToken cancellationToken = default)
        {
            var catalog = await _activeCatalog.LoadAsync(cancellationToken);
            var validation = DataProfileValidator.Validate(content, catalog, forPublish);

            return validation.Issues;
        }
    }
}
