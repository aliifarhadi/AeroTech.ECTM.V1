namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts
{
    public interface IFlightQualityRepository
    {
        Task AddAsync(FlightQuality quality, CancellationToken cancellationToken = default);

        Task<FlightQuality?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<FlightQuality?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FlightQuality>> ListCurrentAsync(IReadOnlyCollection<long> flightIds, CancellationToken cancellationToken = default);
    }
}
