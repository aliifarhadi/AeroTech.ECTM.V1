using AeroTech.ECTM.Application.FlightAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples.Backoffice
{
    public sealed record BackofficeGetFlightSamplesQuery(long FlightId, int? From, int? To) : IRequest<BackofficeFlightSamplesDto>, IFlightSamplesQuery;
}
