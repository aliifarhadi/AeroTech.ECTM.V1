using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight.Backoffice
{
    public sealed record BackofficeReviewFlightCommand(long FlightId, ReviewAction Action, string? Note) : IRequest<BackofficeFlightDetailDto>, IReviewFlightCommand;
}
