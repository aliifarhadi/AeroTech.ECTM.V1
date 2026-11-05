using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Queries.GetFlightRevisions
{
    public sealed class GetFlightRevisionsService : IGetFlightRevisionsService
    {
        private readonly IFlightRepository _flights;
        private readonly IAnalysisRevisionRepository _revisions;

        public GetFlightRevisionsService(IFlightRepository flights, IAnalysisRevisionRepository revisions)
        {
            _flights = flights;
            _revisions = revisions;
        }

        public async Task<IReadOnlyList<BackofficeAnalysisRevisionDto>> ExecuteAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var revisions = await _revisions.ListByFlightAsync(flight.Id, cancellationToken);

            return revisions.Select(BackofficeAnalysisRevisionDto.From).ToList();
        }
    }
}
