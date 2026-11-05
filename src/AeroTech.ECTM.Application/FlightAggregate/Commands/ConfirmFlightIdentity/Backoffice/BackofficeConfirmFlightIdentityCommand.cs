using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity.Backoffice
{
    public sealed record BackofficeConfirmFlightIdentityCommand(
        long FlightId,
        long AircraftId,
        DateOnly FlightDate,
        string FlightNumber,
        IReadOnlyList<EngineChoiceArgs> Engines,
        bool ConfirmDistinctFlight,
        string? Note) : IRequest<BackofficeFlightDetailDto>, IConfirmFlightIdentityCommand;
}
