using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Contracts;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Services
{
    public sealed class FlightReviewer
    {
        private readonly IFlightReviewRepository _reviews;
        private readonly RevisionResults _results;
        private readonly BaselineEvaluationScheduler _baselineEvaluations;
        private readonly IActorResolver _actorResolver;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public FlightReviewer(
            IFlightReviewRepository reviews,
            RevisionResults results,
            BaselineEvaluationScheduler baselineEvaluations,
            IActorResolver actorResolver,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _reviews = reviews;
            _results = results;
            _baselineEvaluations = baselineEvaluations;
            _actorResolver = actorResolver;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task ApplyAsync(Flight flight, ReviewDecision decision, CancellationToken cancellationToken = default)
        {
            var review = await _reviews.GetCurrentAsync(flight.Id, cancellationToken);
            var decides = decision.Action is ReviewAction.ACCEPT or ReviewAction.EXCLUDE;

            if (decides && review is null)
                throw ExceptionFactory.FlightNotAnalysed(flight.Id);

            if (decision.AnalysisRevisionId is { } named && review?.AnalysisRevisionId != named)
                throw ExceptionFactory.ReviewRevisionStale(named, review?.AnalysisRevisionId);

            var now = _clock.GetDateTime();
            var wasAccepted = flight.State == FlightState.ACCEPTED;

            flight.Review(decision.Action, decision.Note, now);

            var entry = new ReviewEntryArgs(decision.Action, _actorResolver.Resolve().Subject, decision.ReasonCode, decision.Note, flight.State);
            review?.Record(_idGenerator.NewId(), entry, now);

            if (flight.State == FlightState.ACCEPTED)
                await _results.MarkFinalAsync(flight.Id, cancellationToken);
            else if (wasAccepted)
                await _results.MarkProvisionalAsync(flight.Id, cancellationToken);

            await _baselineEvaluations.ScheduleForFlightAsync(flight, cancellationToken);
        }
    }
}
