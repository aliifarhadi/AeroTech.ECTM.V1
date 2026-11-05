using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightStabilityPoints.Backoffice
{
    public sealed record BackofficeGetFlightStabilityPointsQuery(long FlightId) : IRequest<BackofficeFlightStabilityPointsDto>;
}
