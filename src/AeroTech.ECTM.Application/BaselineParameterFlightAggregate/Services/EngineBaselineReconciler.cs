using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.ECTM.Domain.BaselineAggregate.Entities;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services
{
    public sealed class EngineBaselineReconciler
    {
        private readonly IBaselineRepository _baselines;
        private readonly IBaselineParameterFlightRepository _evaluations;
        private readonly FlightEvidenceLoader _evidence;
        private readonly ComparisonTolerancesLoader _tolerances;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public EngineBaselineReconciler(
            IBaselineRepository baselines,
            IBaselineParameterFlightRepository evaluations,
            FlightEvidenceLoader evidence,
            ComparisonTolerancesLoader tolerances,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _baselines = baselines;
            _evaluations = evaluations;
            _evidence = evidence;
            _tolerances = tolerances;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task ReconcileAsync(long engineId, CancellationToken cancellationToken = default)
        {
            var baselines = await _baselines.ListEvaluatedForEngineAsync(engineId, cancellationToken);
            var tolerances = await _tolerances.LoadActiveAsync(cancellationToken);
            var evidence = await _evidence.ForEngineAsync(engineId, cancellationToken);
            var finalFlights = evidence.Where(item => item.IsFinal).ToList();
            var current = await _evaluations.ListCurrentForEngineAsync(engineId, cancellationToken);
            var currentByKey = current.ToDictionary(evaluation => (evaluation.BaselineParameterId, evaluation.FlightId));
            var keptIds = new HashSet<long>();
            var recorded = new List<BaselineParameterFlight>();
            var now = _clock.GetDateTime();

            foreach (var flight in finalFlights)
            {
                foreach (var baseline in baselines)
                {
                    foreach (var parameter in baseline.Parameters)
                    {
                        var existing = currentByKey.GetValueOrDefault((parameter.Id, flight.Flight.Id));
                        var fingerprint = EvaluationFingerprint.Of(parameter, flight.Source, tolerances);

                        if (existing is not null && existing.Fingerprint == fingerprint)
                            keptIds.Add(existing.Id);
                        else
                            recorded.Add(Evaluate(baseline, parameter, flight, tolerances, fingerprint, now));
                    }
                }
            }

            await WithdrawAsync(current, keptIds, cancellationToken);
            await RecordAsync(recorded, cancellationToken);
        }

        private BaselineParameterFlight Evaluate(Baseline baseline, BaselineParameter parameter, FlightEvidence flight, ComparisonTolerances tolerances, string fingerprint, DateTimeOffset now)
        {
            var observed = flight.Measure(parameter);
            var comparison = BaselineComparisonEvaluator.Evaluate(baseline, parameter, observed, tolerances);

            return BaselineParameterFlight.Record(_idGenerator.NewId(), baseline, parameter, observed, comparison, fingerprint, now);
        }

        private async Task WithdrawAsync(IReadOnlyList<BaselineParameterFlight> current, IReadOnlySet<long> keptIds, CancellationToken cancellationToken)
        {
            foreach (var evaluation in current.Where(evaluation => !keptIds.Contains(evaluation.Id)))
                evaluation.Withdraw();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task RecordAsync(IReadOnlyList<BaselineParameterFlight> recorded, CancellationToken cancellationToken)
        {
            foreach (var evaluation in recorded)
                await _evaluations.AddAsync(evaluation, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
