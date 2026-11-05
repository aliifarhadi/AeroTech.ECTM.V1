using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightControlContext.Backoffice
{
    public sealed record BackofficeGetFlightControlContextQuery(long FlightId) : IRequest<BackofficeControlContextDto>;
}
