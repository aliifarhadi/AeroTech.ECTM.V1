using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEnginePeriodPerformance
{
    public sealed class GetEnginePeriodPerformanceService : IGetEnginePeriodPerformanceService
    {
        private readonly IEngineRepository _engines;
        private readonly EngineObservationLoader _observations;

        public GetEnginePeriodPerformanceService(IEngineRepository engines, EngineObservationLoader observations)
        {
            _engines = engines;
            _observations = observations;
        }

        public async Task<BackofficePeriodPerformanceDto> ExecuteAsync(long engineId, DateOnly? from, DateOnly? to, SnapshotKind kind, string? selector, CancellationToken cancellationToken = default)
        {
            var engine = await _engines.GetAsync(engineId, cancellationToken)
                         ?? throw ExceptionFactory.EngineNotFound(engineId);
            var loaded = await _observations.LoadAsync(engine.Id, from, to, kind, selector, cancellationToken);
            var performance = PeriodPerformance.Of(loaded.Observations);

            return new BackofficePeriodPerformanceDto(
                engine.Id,
                engine.SerialNumber,
                from,
                to,
                EnumValueDto.Of(kind),
                selector,
                loaded.Flights,
                loaded.AcceptedFlights,
                performance.Contributors,
                performance.Parameters,
                performance.Conditions,
                performance.Configurations);
        }
    }
}
