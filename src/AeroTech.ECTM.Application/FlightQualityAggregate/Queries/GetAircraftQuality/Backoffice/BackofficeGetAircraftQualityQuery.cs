using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetAircraftQuality.Backoffice
{
    public sealed record BackofficeGetAircraftQualityQuery(long AircraftId, DateOnly? From, DateOnly? To) : IRequest<BackofficeAircraftQualityDto>;
}
