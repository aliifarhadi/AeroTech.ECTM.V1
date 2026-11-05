using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Entities;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.FlightAggregate
{
    public sealed class FlightRepository : IFlightRepository
    {
        private readonly EctmDbContext _dbContext;

        public FlightRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(Flight flight, CancellationToken cancellationToken = default)
            => await _dbContext.Flights.AddAsync(flight, cancellationToken);

        public Task<Flight?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.Flights
                .Include(flight => flight.Engines)
                .Include(flight => flight.Issues)
                .Include(flight => flight.Channels)
                .AsSplitQuery()
                .FirstOrDefaultAsync(flight => flight.Id == id, cancellationToken);

        public Task<Flight?> FindAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.Flights
                .AsNoTracking()
                .FirstOrDefaultAsync(flight => flight.Id == id, cancellationToken);

        public Task<Flight?> FindBySourceFileIdAsync(long sourceFileId, CancellationToken cancellationToken = default)
            => _dbContext.Flights
                .AsNoTracking()
                .FirstOrDefaultAsync(flight => flight.SourceFileId == sourceFileId, cancellationToken);

        public async Task<IReadOnlyList<Flight>> ListBySourceFileIdsAsync(IReadOnlyCollection<long> sourceFileIds, CancellationToken cancellationToken = default)
            => await _dbContext.Flights
                .AsNoTracking()
                .Where(flight => sourceFileIds.Contains(flight.SourceFileId))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Flight>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
            => await _dbContext.Flights
                .AsNoTracking()
                .Include(flight => flight.Engines)
                .Where(flight => ids.Contains(flight.Id))
                .ToListAsync(cancellationToken);

        public Task<Flight?> FindSameIdentityAsync(long excludedFlightId, long aircraftId, DateOnly flightDate, string flightNumber, CancellationToken cancellationToken = default)
            => _dbContext.Flights
                .AsNoTracking()
                .Where(flight => flight.Id != excludedFlightId)
                .Where(flight => flight.AircraftId == aircraftId)
                .Where(flight => flight.FlightDate == flightDate)
                .Where(flight => flight.FlightNumber == flightNumber)
                .OrderBy(flight => flight.ImportedAt)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<Flight>> ListForEngineAsync(long engineId, FlightState state, CancellationToken cancellationToken = default)
            => await _dbContext.Flights
                .Include(flight => flight.Engines)
                .Where(flight => flight.State == state)
                .Where(flight => flight.Engines.Any(engine => engine.EngineId == engineId))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Flight>> ListWithEngineBetweenAsync(long engineId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Flights
                .AsNoTracking()
                .Include(flight => flight.Engines)
                .Where(flight => flight.Engines.Any(engine => engine.EngineId == engineId));

            if (from is { } first)
                query = query.Where(flight => flight.FlightDate >= first);

            if (to is { } last)
                query = query.Where(flight => flight.FlightDate <= last);

            return await query
                .OrderBy(flight => flight.FlightDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Flight>> ListForAircraftAsync(long aircraftId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Flights
                .AsNoTracking()
                .Where(flight => flight.AircraftId == aircraftId);

            if (from is { } first)
                query = query.Where(flight => flight.FlightDate >= first);

            if (to is { } last)
                query = query.Where(flight => flight.FlightDate <= last);

            return await query
                .OrderBy(flight => flight.FlightDate)
                .ThenBy(flight => flight.FlightNumber)
                .ToListAsync(cancellationToken);
        }

        public Task<PaginatedList<Flight>> ListAsync(FlightFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Flights
                .AsNoTracking()
                .Include(flight => flight.Engines)
                .AsQueryable();

            if (filter.State is { } state)
                query = query.Where(flight => flight.State == state);

            if (filter.AircraftId is { } aircraftId)
                query = query.Where(flight => flight.AircraftId == aircraftId);

            if (filter.EngineId is { } engineId)
                query = query.Where(flight => flight.Engines.Any(engine => engine.EngineId == engineId));

            if (filter.From is { } from)
                query = query.Where(flight => flight.FlightDate >= from);

            if (filter.To is { } to)
                query = query.Where(flight => flight.FlightDate <= to);

            if (filter.ImportBatchId is { } importBatchId)
                query = query.Where(flight => flight.ImportBatchId == importBatchId);

            if (filter.IdentityStatus is { } identityStatus)
                query = query.Where(flight => flight.IdentityStatus == identityStatus);

            return query
                .OrderByDescending(flight => flight.FlightDate)
                .ThenByDescending(flight => flight.Id)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<IReadOnlyList<FlightChannelPayload>> ListChannelPayloadsAsync(IReadOnlyCollection<long> channelIds, CancellationToken cancellationToken = default)
            => await _dbContext.Set<FlightChannelPayload>()
                .AsNoTracking()
                .Where(payload => channelIds.Contains(payload.Id))
                .ToListAsync(cancellationToken);
    }
}
