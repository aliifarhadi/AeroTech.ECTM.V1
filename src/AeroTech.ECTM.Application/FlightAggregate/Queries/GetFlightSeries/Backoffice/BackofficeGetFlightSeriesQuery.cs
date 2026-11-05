using AeroTech.ECTM.Application.FlightAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries.Backoffice
{
    public sealed record BackofficeGetFlightSeriesQuery(
        long FlightId,
        string Parameters,
        string Sampling,
        int? From,
        int? To,
        int Budget) : IRequest<BackofficeFlightSeriesDto>, IFlightSeriesQuery;
}
