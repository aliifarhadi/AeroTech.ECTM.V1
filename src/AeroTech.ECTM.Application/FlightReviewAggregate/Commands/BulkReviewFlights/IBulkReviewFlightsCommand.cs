using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Commands.BulkReviewFlights
{
    public interface IBulkReviewFlightsCommand
    {
        IReadOnlyList<long> FlightIds { get; }

        ReviewAction Action { get; }

        ExclusionReason? ReasonCode { get; }

        string? Note { get; }
    }
}
