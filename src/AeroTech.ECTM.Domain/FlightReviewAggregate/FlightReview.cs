using AeroTech.ECTM.Domain.FlightReviewAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Entities;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightReviewAggregate
{
    public sealed class FlightReview : AggregateRoot<long>
    {
        private readonly List<FlightReviewEntry> _entries = new();

        private FlightReview()
        {
        }

        private FlightReview(long id, long flightId, long analysisRevisionId, DateTimeOffset openedAt)
        {
            Id = id;
            FlightId = flightId;
            AnalysisRevisionId = analysisRevisionId;
            State = ReviewState.PENDING;
            OpenedAt = openedAt;
        }

        public long FlightId { get; private set; }

        public long AnalysisRevisionId { get; private set; }

        public ReviewState State { get; private set; }

        public string? DecidedBy { get; private set; }

        public DateTimeOffset? DecidedAt { get; private set; }

        public ExclusionReason? ReasonCode { get; private set; }

        public string? Note { get; private set; }

        public DateTimeOffset OpenedAt { get; private set; }

        public IReadOnlyCollection<FlightReviewEntry> Entries => _entries.AsReadOnly();

        public static FlightReview Open(long id, long flightId, long analysisRevisionId, DateTimeOffset now)
            => new(id, flightId, analysisRevisionId, now);

        public void Record(long entryId, ReviewEntryArgs entry, DateTimeOffset now)
        {
            _entries.Add(new FlightReviewEntry(entryId, Id, entry.Action, entry.Actor, entry.ReasonCode, entry.Note, entry.FlightStateAfter, now));

            if (entry.FlightStateAfter == FlightState.ACCEPTED)
                Decide(ReviewState.ACCEPTED, entry, now);
            else if (entry.FlightStateAfter == FlightState.EXCLUDED)
                Decide(ReviewState.EXCLUDED, entry, now);
            else if (State == ReviewState.ACCEPTED)
                Reopen();
        }

        public void Supersede()
        {
            State = ReviewState.SUPERSEDED;
        }

        private void Decide(ReviewState state, ReviewEntryArgs entry, DateTimeOffset now)
        {
            State = state;
            DecidedBy = entry.Actor;
            DecidedAt = now;
            ReasonCode = entry.ReasonCode;
            Note = entry.Note;
        }

        private void Reopen()
        {
            State = ReviewState.PENDING;
            DecidedBy = null;
            DecidedAt = null;
            ReasonCode = null;
            Note = null;
        }
    }
}
