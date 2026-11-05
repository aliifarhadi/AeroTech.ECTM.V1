using AeroTech.ECTM.Application.FlightAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight
{
    public interface IReviewFlightService
    {
        Task<BackofficeFlightDetailDto> ReviewAsync(IReviewFlightCommand command, CancellationToken cancellationToken = default);
    }
}
