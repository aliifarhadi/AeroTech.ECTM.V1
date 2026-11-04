using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds
{
    public interface IGetConfigurationKindsService
    {
        Task<IReadOnlyList<BackofficeConfigurationKindDto>> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
