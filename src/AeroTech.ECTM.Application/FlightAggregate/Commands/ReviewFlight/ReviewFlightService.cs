using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight
{
    public sealed class ReviewFlightService : IReviewFlightService
    {
        private readonly IFlightRepository _flights;
        private readonly FlightDtoLoader _flightDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public ReviewFlightService(IFlightRepository flights, FlightDtoLoader flightDtos, IUnitOfWork unitOfWork, IClock clock)
        {
            _flights = flights;
            _flightDtos = flightDtos;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task<BackofficeFlightDetailDto> ReviewAsync(IReviewFlightCommand command, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(command.FlightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(command.FlightId);

            flight.Review(command.Action, command.Note, _clock.GetDateTime());

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _flightDtos.LoadDetailAsync(flight, cancellationToken);
        }
    }
}
