using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight
{
    public interface IReviewFlightCommand
    {
        long FlightId { get; }

        ReviewAction Action { get; }

        ExclusionReason? ReasonCode { get; }

        string? Note { get; }

        long? AnalysisRevisionId { get; }
    }
}
