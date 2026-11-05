using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Queries.GetFlightRevisions.Backoffice
{
    public sealed record BackofficeGetFlightRevisionsQuery(long FlightId) : IRequest<IReadOnlyList<BackofficeAnalysisRevisionDto>>;
}
