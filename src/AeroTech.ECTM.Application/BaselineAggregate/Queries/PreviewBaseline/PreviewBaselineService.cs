using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.PreviewBaseline
{
    public sealed class PreviewBaselineService : IPreviewBaselineService
    {
        private const long NotStored = 0;

        private readonly IBaselineRepository _baselines;
        private readonly BaselineDtoLoader _baselineDtos;
        private readonly FlightEvidenceLoader _evidence;
        private readonly ComparisonTolerancesLoader _tolerances;
        private readonly IClock _clock;

        public PreviewBaselineService(
            IBaselineRepository baselines,
            BaselineDtoLoader baselineDtos,
            FlightEvidenceLoader evidence,
            ComparisonTolerancesLoader tolerances,
            IClock clock)
        {
            _baselines = baselines;
            _baselineDtos = baselineDtos;
            _evidence = evidence;
            _tolerances = tolerances;
            _clock = clock;
        }

        public async Task<BackofficeBaselinePreviewDto> ExecuteAsync(long baselineId, long? flightId, CancellationToken cancellationToken = default)
        {
            var baseline = await _baselines.FindAsync(baselineId, cancellationToken)
                           ?? throw ExceptionFactory.BaselineNotFound(baselineId);

            var baselineDto = await _baselineDtos.LoadAsync(baseline, cancellationToken);
            var issues = baseline.PublicationIssues();
            var canPublish = baseline.State == BaselineState.DRAFT && issues.Count == 0;
            var issueDtos = issues.Select(issue => EnumValueDto.Of(issue)).ToList();
            var flight = await PreviewFlightOfAsync(baseline.EngineId, flightId, cancellationToken);

            if (flight is null)
                return new BackofficeBaselinePreviewDto(baselineDto, canPublish, issueDtos, null, null, null, []);

            var evaluations = await EvaluateAsync(baseline, flight, cancellationToken);

            return new BackofficeBaselinePreviewDto(baselineDto, canPublish, issueDtos, flight.Flight.Id, flight.Flight.FlightNumber, flight.Flight.FlightDate, evaluations);
        }

        private async Task<FlightEvidence?> PreviewFlightOfAsync(long engineId, long? flightId, CancellationToken cancellationToken)
        {
            var evidence = await _evidence.ForEngineAsync(engineId, cancellationToken);

            if (flightId is not { } requested)
                return evidence.LastOrDefault(item => item.IsFinal);

            return evidence.FirstOrDefault(item => item.Flight.Id == requested)
                   ?? throw ExceptionFactory.FlightNotFound(requested);
        }

        private async Task<IReadOnlyList<BackofficeBaselineEvaluationDto>> EvaluateAsync(Baseline baseline, FlightEvidence flight, CancellationToken cancellationToken)
        {
            var tolerances = await _tolerances.LoadActiveAsync(cancellationToken);
            var now = _clock.GetDateTime();
            var evaluations = new List<BackofficeBaselineEvaluationDto>();

            foreach (var parameter in baseline.Parameters.Where(parameter => parameter.RawValue is not null))
            {
                var observed = flight.Measure(parameter);
                var comparison = BaselineComparisonEvaluator.Evaluate(baseline, parameter, observed, tolerances);
                var fingerprint = EvaluationFingerprint.Of(parameter, flight.Source, tolerances);
                var evaluation = BaselineParameterFlight.Record(NotStored, baseline, parameter, observed, comparison, fingerprint, now);
                var status = flight.IsFinal ? evaluation.Status : ComparisonStatus.NOT_FINAL;

                evaluations.Add(BackofficeBaselineEvaluationDto.From(evaluation, null, baseline, flight.Flight.FlightNumber, false, status));
            }

            return evaluations;
        }
    }
}
