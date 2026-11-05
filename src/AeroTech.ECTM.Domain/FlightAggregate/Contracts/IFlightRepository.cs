using AeroTech.Messages.Ectm.Enums;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Entities;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Domain.FlightAggregate.Contracts
{
    public interface IFlightRepository
    {
        Task AddAsync(Flight flight, CancellationToken cancellationToken = default);

        Task<Flight?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<Flight?> FindAsync(long id, CancellationToken cancellationToken = default);

        Task<Flight?> FindBySourceFileIdAsync(long sourceFileId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Flight>> ListBySourceFileIdsAsync(IReadOnlyCollection<long> sourceFileIds, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Flight>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

        Task<Flight?> FindSameIdentityAsync(long excludedFlightId, long aircraftId, DateOnly flightDate, string flightNumber, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Flight>> ListForEngineAsync(long engineId, FlightState state, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Flight>> ListWithEngineBetweenAsync(long engineId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Flight>> ListForAircraftAsync(long aircraftId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);

        Task<PaginatedList<Flight>> ListAsync(FlightFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FlightChannelPayload>> ListChannelPayloadsAsync(IReadOnlyCollection<long> channelIds, CancellationToken cancellationToken = default);
    }
}
