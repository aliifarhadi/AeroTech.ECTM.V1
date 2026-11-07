using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.PreviewBaseline.Backoffice
{
    public sealed class BackofficePreviewBaselineQueryHandler : IRequestHandler<BackofficePreviewBaselineQuery, BackofficeBaselinePreviewDto>
    {
        private readonly IPreviewBaselineService _service;

        public BackofficePreviewBaselineQueryHandler(IPreviewBaselineService service) => _service = service;

        public Task<BackofficeBaselinePreviewDto> Handle(BackofficePreviewBaselineQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.BaselineId, query.FlightId, cancellationToken);
    }
}
