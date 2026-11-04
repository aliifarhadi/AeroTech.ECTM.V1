using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public interface IConfigurationContentValidator
    {
        ConfigurationKind Kind { get; }

        Task<IReadOnlyList<ValidationIssue>> ValidateAsync(string content, bool forPublish, CancellationToken cancellationToken = default);
    }
}
