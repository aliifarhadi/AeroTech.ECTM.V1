using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Queries.GetReviewReport
{
    public interface IGetReviewReportService
    {
        Task<BackofficeReviewReportDto> ExecuteAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    }
}
