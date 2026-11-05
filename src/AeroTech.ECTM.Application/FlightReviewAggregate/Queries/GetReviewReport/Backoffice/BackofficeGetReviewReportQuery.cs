using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Queries.GetReviewReport.Backoffice
{
    public sealed record BackofficeGetReviewReportQuery(DateOnly From, DateOnly To) : IRequest<BackofficeReviewReportDto>;
}
