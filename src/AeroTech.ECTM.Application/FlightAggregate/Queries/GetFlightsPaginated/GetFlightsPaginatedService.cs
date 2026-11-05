using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightsPaginated
{
    public sealed class GetFlightsPaginatedService : IGetFlightsPaginatedService
    {
        private readonly IFlightRepository _flights;
        private readonly FlightDtoLoader _flightDtos;

        public GetFlightsPaginatedService(IFlightRepository flights, FlightDtoLoader flightDtos)
        {
            _flights = flights;
            _flightDtos = flightDtos;
        }

        public async Task<PaginatedList<BackofficeFlightSummaryDto>> ExecuteAsync(IFlightsPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var filter = new FlightFilterArgs(query.State, query.AircraftId, query.EngineId, query.From, query.To, query.BatchId, query.IdentityStatus);
            var page = await _flights.ListAsync(filter, query.PageNumber, query.PageSize, cancellationToken);

            var summaries = await _flightDtos.LoadSummariesAsync(page.Results.ToList(), cancellationToken);
            var summaryById = summaries.ToDictionary(summary => summary.Id);

            return page.Map(flight => summaryById[flight.Id]);
        }
    }
}
