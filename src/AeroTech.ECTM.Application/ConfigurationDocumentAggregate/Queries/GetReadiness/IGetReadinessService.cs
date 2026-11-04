using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetReadiness
{
    public interface IGetReadinessService
    {
        Task<BackofficeReadinessDto> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
