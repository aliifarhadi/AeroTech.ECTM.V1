using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Contracts;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.BaselineParameterFlightAggregate
{
    public sealed class BaselineParameterFlightRepository : IBaselineParameterFlightRepository
    {
        private readonly EctmDbContext _dbContext;

        public BaselineParameterFlightRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(BaselineParameterFlight evaluation, CancellationToken cancellationToken = default)
            => await _dbContext.BaselineParameterFlights.AddAsync(evaluation, cancellationToken);

        public async Task<IReadOnlyList<BaselineParameterFlight>> ListCurrentForEngineAsync(long engineId, CancellationToken cancellationToken = default)
            => await _dbContext.BaselineParameterFlights
                .Where(evaluation => evaluation.EngineId == engineId)
                .Where(evaluation => evaluation.IsCurrent)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<long>> ListEngineIdsWithCurrentForFlightAsync(long flightId, CancellationToken cancellationToken = default)
            => await _dbContext.BaselineParameterFlights
                .Where(evaluation => evaluation.FlightId == flightId)
                .Where(evaluation => evaluation.IsCurrent)
                .Select(evaluation => evaluation.EngineId)
                .Distinct()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<BaselineParameterFlight>> ListCurrentForFlightAsync(long flightId, EvaluationFilterArgs filter, CancellationToken cancellationToken = default)
        {
            var query = CurrentMatching(filter).Where(evaluation => evaluation.FlightId == flightId);

            return await query
                .OrderBy(evaluation => evaluation.Position)
                .ThenBy(evaluation => evaluation.SnapshotKind)
                .ThenBy(evaluation => evaluation.Selector)
                .ThenBy(evaluation => evaluation.ParameterCode)
                .ThenBy(evaluation => evaluation.BaselineId)
                .ToListAsync(cancellationToken);
        }

        public Task<PaginatedList<BaselineParameterFlight>> ListTrendAsync(long engineId, EvaluationFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = CurrentMatching(filter).Where(evaluation => evaluation.EngineId == engineId);

            return query
                .OrderBy(evaluation => evaluation.FlightDate)
                .ThenBy(evaluation => evaluation.FlightId)
                .ThenBy(evaluation => evaluation.BaselineId)
                .ThenBy(evaluation => evaluation.ParameterCode)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }

        private IQueryable<BaselineParameterFlight> CurrentMatching(EvaluationFilterArgs filter)
        {
            var query = _dbContext.BaselineParameterFlights
                .AsNoTracking()
                .Where(evaluation => evaluation.IsCurrent);

            if (filter.BaselineIds is { Count: > 0 } baselineIds)
                query = query.Where(evaluation => baselineIds.Contains(evaluation.BaselineId));

            if (filter.SnapshotKind is { } snapshotKind)
                query = query.Where(evaluation => evaluation.SnapshotKind == snapshotKind);

            if (filter.Selector is { } selector)
                query = query.Where(evaluation => evaluation.Selector == selector);

            if (filter.ParameterCode is { } parameterCode)
                query = query.Where(evaluation => evaluation.ParameterCode == parameterCode);

            if (filter.From is { } from)
                query = query.Where(evaluation => evaluation.FlightDate >= from);

            if (filter.To is { } to)
                query = query.Where(evaluation => evaluation.FlightDate <= to);

            return query;
        }
    }
}
