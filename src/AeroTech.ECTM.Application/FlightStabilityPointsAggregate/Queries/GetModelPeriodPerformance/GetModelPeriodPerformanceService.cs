using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEnginePeriodPerformance;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetModelPeriodPerformance
{
    public sealed class GetModelPeriodPerformanceService : IGetModelPeriodPerformanceService
    {
        private const int MaximumEngines = 500;

        private readonly IEngineModelRepository _models;
        private readonly IEngineRepository _engines;
        private readonly IGetEnginePeriodPerformanceService _enginePerformance;

        public GetModelPeriodPerformanceService(IEngineModelRepository models, IEngineRepository engines, IGetEnginePeriodPerformanceService enginePerformance)
        {
            _models = models;
            _engines = engines;
            _enginePerformance = enginePerformance;
        }

        public async Task<BackofficeModelPeriodPerformanceDto> ExecuteAsync(long engineModelId, DateOnly? from, DateOnly? to, SnapshotKind kind, string? selector, CancellationToken cancellationToken = default)
        {
            if (await _models.GetAsync(engineModelId, cancellationToken) is null)
                throw ExceptionFactory.EngineModelNotFound(engineModelId);

            var engines = await _engines.ListAsync(null, engineModelId, null, 1, MaximumEngines, cancellationToken);
            var performances = new List<BackofficePeriodPerformanceDto>();

            foreach (var engine in engines.Results)
                performances.Add(await _enginePerformance.ExecuteAsync(engine.Id, from, to, kind, selector, cancellationToken));

            return new BackofficeModelPeriodPerformanceDto(engineModelId, from, to, EnumValueDto.Of(kind), selector, performances, SpreadOf(performances));
        }

        private static List<BackofficeParameterSpreadDto> SpreadOf(IReadOnlyList<BackofficePeriodPerformanceDto> performances)
            => performances
                .SelectMany(performance => performance.Parameters)
                .Where(distribution => distribution.Mean is not null)
                .GroupBy(distribution => distribution.Parameter)
                .Select(group =>
                {
                    var lowest = group.Min(distribution => distribution.Mean);
                    var highest = group.Max(distribution => distribution.Mean);

                    return new BackofficeParameterSpreadDto(group.Key, group.Count(), lowest, highest, highest - lowest);
                })
                .ToList();
    }
}
