using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Queries.GetFlightRevisions.Backoffice
{
    public sealed class BackofficeGetFlightRevisionsQueryHandler : IRequestHandler<BackofficeGetFlightRevisionsQuery, IReadOnlyList<BackofficeAnalysisRevisionDto>>
    {
        private readonly IGetFlightRevisionsService _service;

        public BackofficeGetFlightRevisionsQueryHandler(IGetFlightRevisionsService service) => _service = service;

        public Task<IReadOnlyList<BackofficeAnalysisRevisionDto>> Handle(BackofficeGetFlightRevisionsQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, cancellationToken);
    }
}
