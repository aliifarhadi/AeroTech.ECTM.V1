using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Commands.BulkReviewFlights
{
    public interface IBulkReviewFlightsService
    {
        Task<BackofficeBulkReviewDto> ReviewAsync(IBulkReviewFlightsCommand command, CancellationToken cancellationToken = default);
    }
}
