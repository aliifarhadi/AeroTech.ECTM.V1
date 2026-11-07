using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;

namespace AeroTech.ECTM.Application.BaselineAggregate.Services
{
    public sealed class BaselineDtoLoader
    {
        private readonly IEngineRepository _engines;
        private readonly IBaselinePrimarySelectionRepository _primarySelections;

        public BaselineDtoLoader(IEngineRepository engines, IBaselinePrimarySelectionRepository primarySelections)
        {
            _engines = engines;
            _primarySelections = primarySelections;
        }

        public async Task<IReadOnlyList<BackofficeBaselineDto>> LoadForEngineAsync(long engineId, IReadOnlyList<Baseline> baselines, CancellationToken cancellationToken = default)
        {
            var serialNumber = await SerialNumberOfAsync(engineId, cancellationToken);
            var primaryParameterIds = await PrimaryParameterIdsAsync(engineId, cancellationToken);

            return baselines
                .Select(baseline => BackofficeBaselineDto.From(baseline, serialNumber, primaryParameterIds))
                .ToList();
        }

        public async Task<BackofficeBaselineDto> LoadAsync(Baseline baseline, CancellationToken cancellationToken = default)
        {
            var dtos = await LoadForEngineAsync(baseline.EngineId, [baseline], cancellationToken);

            return dtos.Single();
        }

        public async Task<string?> SerialNumberOfAsync(long engineId, CancellationToken cancellationToken = default)
        {
            var engine = await _engines.GetAsync(engineId, cancellationToken);

            return engine?.SerialNumber;
        }

        private async Task<IReadOnlySet<long>> PrimaryParameterIdsAsync(long engineId, CancellationToken cancellationToken)
        {
            var selections = await _primarySelections.ListForEngineAsync(engineId, cancellationToken);

            return selections.Select(selection => selection.BaselineParameterId).ToHashSet();
        }
    }
}
