using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById.Backoffice
{
    public sealed record BackofficeGetAircraftByIdQuery(long AircraftId) : IRequest<BackofficeAircraftDto>;
}
