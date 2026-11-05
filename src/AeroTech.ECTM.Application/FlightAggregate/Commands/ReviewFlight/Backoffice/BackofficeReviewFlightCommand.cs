using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight.Backoffice
{
    public sealed record BackofficeReviewFlightCommand(long FlightId, ReviewAction Action, ExclusionReason? ReasonCode, string? Note, long? AnalysisRevisionId) : IRequest<BackofficeFlightDetailDto>, IReviewFlightCommand;
}
