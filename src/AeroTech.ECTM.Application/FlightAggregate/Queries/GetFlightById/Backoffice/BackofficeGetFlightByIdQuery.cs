using AeroTech.ECTM.Application.FlightAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightById.Backoffice
{
    public sealed record BackofficeGetFlightByIdQuery(long FlightId) : IRequest<BackofficeFlightDetailDto>;
}
