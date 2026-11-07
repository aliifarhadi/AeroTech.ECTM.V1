using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Contracts
{
    public interface IBaselineParameterFlightRepository
    {
        Task AddAsync(BaselineParameterFlight evaluation, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BaselineParameterFlight>> ListCurrentForEngineAsync(long engineId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<long>> ListEngineIdsWithCurrentForFlightAsync(long flightId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BaselineParameterFlight>> ListCurrentForFlightAsync(long flightId, EvaluationFilterArgs filter, CancellationToken cancellationToken = default);

        Task<PaginatedList<BaselineParameterFlight>> ListTrendAsync(long engineId, EvaluationFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
