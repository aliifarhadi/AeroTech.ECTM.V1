using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate;
using AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services
{
    public sealed class BaselineEvaluationDtoLoader
    {
        private readonly IBaselineRepository _baselines;
        private readonly IBaselinePrimarySelectionRepository _primarySelections;
        private readonly IFlightRepository _flights;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;

        public BaselineEvaluationDtoLoader(
            IBaselineRepository baselines,
            IBaselinePrimarySelectionRepository primarySelections,
            IFlightRepository flights,
            IFlightStabilityPointsRepository stabilityPoints)
        {
            _baselines = baselines;
            _primarySelections = primarySelections;
            _flights = flights;
            _stabilityPoints = stabilityPoints;
        }

        public async Task<IReadOnlyList<BackofficeBaselineEvaluationDto>> LoadAsync(IReadOnlyList<BaselineParameterFlight> evaluations, CancellationToken cancellationToken = default)
        {
            if (evaluations.Count == 0)
                return [];

            var baselineIds = evaluations.Select(evaluation => evaluation.BaselineId).Distinct().ToList();
            var baselines = await _baselines.ListByIdsAsync(baselineIds, cancellationToken);
            var baselineById = baselines.ToDictionary(baseline => baseline.Id);

            var flightIds = evaluations.Select(evaluation => evaluation.FlightId).Distinct().ToList();
            var flights = await _flights.ListByIdsAsync(flightIds, cancellationToken);
            var flightById = flights.ToDictionary(flight => flight.Id);

            var currentPoints = await _stabilityPoints.ListCurrentAsync(flightIds, cancellationToken);
            var currentRevisionByFlight = currentPoints.ToDictionary(points => points.FlightId, points => (long?)points.AnalysisRevisionId);

            var primaryParameterIds = await PrimaryParameterIdsAsync(evaluations, cancellationToken);
            var dtos = new List<BackofficeBaselineEvaluationDto>();

            foreach (var evaluation in evaluations)
            {
                var flight = flightById[evaluation.FlightId];
                var currentRevision = currentRevisionByFlight.GetValueOrDefault(evaluation.FlightId);
                var status = evaluation.StatusFor(flight.State, currentRevision);
                var isPrimary = primaryParameterIds.Contains(evaluation.BaselineParameterId);
                var baseline = baselineById[evaluation.BaselineId];

                dtos.Add(BackofficeBaselineEvaluationDto.From(evaluation, evaluation.Id, baseline, flight.FlightNumber, isPrimary, status));
            }

            return dtos;
        }

        private async Task<IReadOnlySet<long>> PrimaryParameterIdsAsync(IReadOnlyList<BaselineParameterFlight> evaluations, CancellationToken cancellationToken)
        {
            var primaryParameterIds = new HashSet<long>();

            foreach (var engineId in evaluations.Select(evaluation => evaluation.EngineId).Distinct())
            {
                var selections = await _primarySelections.ListForEngineAsync(engineId, cancellationToken);

                primaryParameterIds.UnionWith(selections.Select(selection => selection.BaselineParameterId));
            }

            return primaryParameterIds;
        }
    }
}
