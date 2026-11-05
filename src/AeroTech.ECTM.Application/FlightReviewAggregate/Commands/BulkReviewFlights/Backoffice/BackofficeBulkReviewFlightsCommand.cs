using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Commands.BulkReviewFlights.Backoffice
{
    public sealed record BackofficeBulkReviewFlightsCommand(IReadOnlyList<long> FlightIds, ReviewAction Action, ExclusionReason? ReasonCode, string? Note)
        : IRequest<BackofficeBulkReviewDto>, IBulkReviewFlightsCommand;
}
