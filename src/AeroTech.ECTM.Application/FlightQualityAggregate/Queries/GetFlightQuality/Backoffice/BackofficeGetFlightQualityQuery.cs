using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetFlightQuality.Backoffice
{
    public sealed record BackofficeGetFlightQualityQuery(long FlightId) : IRequest<BackofficeFlightQualityDto>;
}
