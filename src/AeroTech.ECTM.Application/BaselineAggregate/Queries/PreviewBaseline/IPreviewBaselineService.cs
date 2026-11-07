using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.PreviewBaseline
{
    public interface IPreviewBaselineService
    {
        Task<BackofficeBaselinePreviewDto> ExecuteAsync(long baselineId, long? flightId, CancellationToken cancellationToken = default);
    }
}
