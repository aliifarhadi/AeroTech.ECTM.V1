namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts
{
    public interface IFlightStabilityPointsRepository
    {
        Task AddAsync(FlightStabilityPoints points, CancellationToken cancellationToken = default);

        Task<FlightStabilityPoints?> GetCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<FlightStabilityPoints?> FindCurrentAsync(long flightId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FlightStabilityPoints>> ListCurrentAsync(IReadOnlyCollection<long> flightIds, CancellationToken cancellationToken = default);
    }
}
