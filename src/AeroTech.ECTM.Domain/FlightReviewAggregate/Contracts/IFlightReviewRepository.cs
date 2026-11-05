namespace AeroTech.ECTM.Domain.FlightReviewAggregate.Contracts
{
    public interface IFlightReviewRepository
    {
        Task AddAsync(FlightReview review, CancellationToken cancellationToken = default);

        Task<FlightReview?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<FlightReview?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FlightReview>> ListByFlightAsync(long flightId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FlightReview>> ListWithEntriesBetweenAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    }
}
