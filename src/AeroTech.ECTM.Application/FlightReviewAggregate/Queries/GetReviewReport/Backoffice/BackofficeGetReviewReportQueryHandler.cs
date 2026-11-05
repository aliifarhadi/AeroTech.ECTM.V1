using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Queries.GetReviewReport.Backoffice
{
    public sealed class BackofficeGetReviewReportQueryHandler : IRequestHandler<BackofficeGetReviewReportQuery, BackofficeReviewReportDto>
    {
        private readonly IGetReviewReportService _service;

        public BackofficeGetReviewReportQueryHandler(IGetReviewReportService service) => _service = service;

        public Task<BackofficeReviewReportDto> Handle(BackofficeGetReviewReportQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.From, query.To, cancellationToken);
    }
}
