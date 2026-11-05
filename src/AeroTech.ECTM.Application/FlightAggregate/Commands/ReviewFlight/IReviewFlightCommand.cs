using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight
{
    public interface IReviewFlightCommand
    {
        long FlightId { get; }

        ReviewAction Action { get; }

        string? Note { get; }
    }
}
