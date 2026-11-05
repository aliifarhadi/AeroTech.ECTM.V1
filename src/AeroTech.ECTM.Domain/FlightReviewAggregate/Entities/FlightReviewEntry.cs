using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightReviewAggregate.Entities
{
    public sealed class FlightReviewEntry : Entity<long>
    {
        private FlightReviewEntry()
        {
        }

        internal FlightReviewEntry(long id, long flightReviewId, ReviewAction action, string? actor, ExclusionReason? reasonCode, string? note, FlightState flightStateAfter, DateTimeOffset at)
        {
            Id = id;
            FlightReviewId = flightReviewId;
            Action = action;
            Actor = actor;
            ReasonCode = reasonCode;
            Note = note;
            FlightStateAfter = flightStateAfter;
            At = at;
        }

        public long FlightReviewId { get; private set; }

        public ReviewAction Action { get; private set; }

        public string? Actor { get; private set; }

        public ExclusionReason? ReasonCode { get; private set; }

        public string? Note { get; private set; }

        public FlightState FlightStateAfter { get; private set; }

        public DateTimeOffset At { get; private set; }
    }
}
