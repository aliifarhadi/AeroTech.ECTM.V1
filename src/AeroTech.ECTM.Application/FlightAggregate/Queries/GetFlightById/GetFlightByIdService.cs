using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightById
{
    public sealed class GetFlightByIdService : IGetFlightByIdService
    {
        private readonly IFlightRepository _flights;
        private readonly FlightDtoLoader _flightDtos;

        public GetFlightByIdService(IFlightRepository flights, FlightDtoLoader flightDtos)
        {
            _flights = flights;
            _flightDtos = flightDtos;
        }

        public async Task<BackofficeFlightDetailDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);

            return await _flightDtos.LoadDetailAsync(flight, cancellationToken);
        }
    }
}
