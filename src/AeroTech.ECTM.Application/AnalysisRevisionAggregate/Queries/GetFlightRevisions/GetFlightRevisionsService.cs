using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Contracts;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Queries.GetFlightRevisions
{
    public sealed class GetFlightRevisionsService : IGetFlightRevisionsService
    {
        private readonly IFlightRepository _flights;
        private readonly IAnalysisRevisionRepository _revisions;
        private readonly IFlightReviewRepository _reviews;

        public GetFlightRevisionsService(IFlightRepository flights, IAnalysisRevisionRepository revisions, IFlightReviewRepository reviews)
        {
            _flights = flights;
            _revisions = revisions;
            _reviews = reviews;
        }

        public async Task<IReadOnlyList<BackofficeAnalysisRevisionDto>> ExecuteAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var revisions = await _revisions.ListByFlightAsync(flight.Id, cancellationToken);
            var reviews = await _reviews.ListByFlightAsync(flight.Id, cancellationToken);
            var reviewByRevision = reviews.ToDictionary(review => review.AnalysisRevisionId);

            return revisions
                .Select(revision => BackofficeAnalysisRevisionDto.From(revision, reviewByRevision.GetValueOrDefault(revision.Id)))
                .ToList();
        }
    }
}
