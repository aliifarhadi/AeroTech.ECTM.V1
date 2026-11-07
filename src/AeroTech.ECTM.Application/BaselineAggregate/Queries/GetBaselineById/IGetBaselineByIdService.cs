using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.GetBaselineById
{
    public interface IGetBaselineByIdService
    {
        Task<BackofficeBaselineDto> ExecuteAsync(long baselineId, CancellationToken cancellationToken = default);
    }
}
