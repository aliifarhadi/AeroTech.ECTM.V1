using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class ParameterCatalogContentValidator : IConfigurationContentValidator
    {
        public const string ActiveProfileIncompatible = "ACTIVE_PROFILE_INCOMPATIBLE";

        private readonly IConfigurationDocumentRepository _documents;

        public ParameterCatalogContentValidator(IConfigurationDocumentRepository documents) => _documents = documents;

        public ConfigurationKind Kind => ConfigurationKind.PARAMETER_CATALOG;

        public async Task<IReadOnlyList<ValidationIssue>> ValidateAsync(string content, bool forPublish, CancellationToken cancellationToken = default)
        {
            var validation = ParameterCatalogValidator.Validate(content);
            var issues = validation.Issues.ToList();

            if (!forPublish || !validation.IsValid)
                return issues;

            var activeProfile = await _documents.FindActiveAsync(ConfigurationKind.DATA_PROFILE, cancellationToken);

            if (activeProfile is null)
                return issues;

            var profileValidation = DataProfileValidator.Validate(activeProfile.Content, validation.Catalog, forPublish: true);

            foreach (var profileIssue in profileValidation.Issues)
            {
                var message = $"The active DATA_PROFILE v{activeProfile.VersionNumber} would break: {profileIssue.Path} {profileIssue.Message}";
                issues.Add(new ValidationIssue("$", ActiveProfileIncompatible, message));
            }

            return issues;
        }
    }
}
