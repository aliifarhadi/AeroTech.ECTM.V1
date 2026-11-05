using AeroTech.ECTM.Domain.FlightAggregate.Arguments;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity
{
    public interface IConfirmFlightIdentityCommand
    {
        long FlightId { get; }

        long AircraftId { get; }

        DateOnly FlightDate { get; }

        string FlightNumber { get; }

        IReadOnlyList<EngineChoiceArgs> Engines { get; }

        bool ConfirmDistinctFlight { get; }

        string? Note { get; }
    }
}
