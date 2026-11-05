using AeroTech.ECTM.Application.FlightReviewAggregate.Services;
using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight
{
    public sealed class ReviewFlightService : IReviewFlightService
    {
        private readonly IFlightRepository _flights;
        private readonly FlightReviewer _reviewer;
        private readonly FlightDtoLoader _flightDtos;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewFlightService(IFlightRepository flights, FlightReviewer reviewer, FlightDtoLoader flightDtos, IUnitOfWork unitOfWork)
        {
            _flights = flights;
            _reviewer = reviewer;
            _flightDtos = flightDtos;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeFlightDetailDto> ReviewAsync(IReviewFlightCommand command, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(command.FlightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(command.FlightId);

            await _reviewer.ApplyAsync(flight, new ReviewDecision(command.Action, command.ReasonCode, command.Note, command.AnalysisRevisionId), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _flightDtos.LoadDetailAsync(flight, cancellationToken);
        }
    }
}
