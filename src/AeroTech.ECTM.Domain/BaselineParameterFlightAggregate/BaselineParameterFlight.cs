using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineAggregate.Entities;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate
{
    public sealed class BaselineParameterFlight : AggregateRoot<long>
    {
        private BaselineParameterFlight()
        {
        }

        private BaselineParameterFlight(long id, Baseline baseline, BaselineParameter parameter, ObservedMeasurement observed, BaselineComparison comparison, string fingerprint, DateTimeOffset now)
        {
            Id = id;
            BaselineId = baseline.Id;
            BaselineParameterId = parameter.Id;
            EngineId = observed.Source.EngineId;
            FlightId = observed.Source.FlightId;
            FlightDate = observed.Source.FlightDate;
            AnalysisRevisionId = observed.Source.AnalysisRevisionId;
            FlightStabilityPointsId = observed.Source.FlightStabilityPointsId;
            Position = (byte)observed.Source.Position;
            SnapshotKind = parameter.SnapshotKind;
            Selector = parameter.Selector;
            Statistic = parameter.Statistic;
            ParameterCode = parameter.ParameterCode;
            Unit = parameter.Unit;
            SnapshotRulesDocumentId = observed.Source.SnapshotRulesDocumentId;
            SnapshotRulesVersion = observed.Source.SnapshotRulesVersion;
            SourceStartIndex = observed.StartIndex;
            SourceEndIndexInclusive = observed.EndIndexInclusive;
            SourcePeakIndex = observed.PeakIndex;
            RawObserved = comparison.RawObserved;
            BaselineRawValue = parameter.RawValue!.Value;
            RawDelta = comparison.RawDelta;
            Status = comparison.Status;
            Reasons = comparison.Reasons.ToList();
            Detail = comparison.Detail.ToJson();
            Fingerprint = fingerprint;
            CalculationVersion = BaselineComparisonEvaluator.CalculationVersion;
            IsCurrent = true;
            CalculatedAt = now;
        }

        public long BaselineId { get; private set; }

        public long BaselineParameterId { get; private set; }

        public long EngineId { get; private set; }

        public long FlightId { get; private set; }

        public DateOnly? FlightDate { get; private set; }

        public long AnalysisRevisionId { get; private set; }

        public long FlightStabilityPointsId { get; private set; }

        public byte Position { get; private set; }

        public SnapshotKind SnapshotKind { get; private set; }

        public string? Selector { get; private set; }

        public BaselineStatistic Statistic { get; private set; }

        public string ParameterCode { get; private set; } = default!;

        public string? Unit { get; private set; }

        public long SnapshotRulesDocumentId { get; private set; }

        public int SnapshotRulesVersion { get; private set; }

        public int? SourceStartIndex { get; private set; }

        public int? SourceEndIndexInclusive { get; private set; }

        public int? SourcePeakIndex { get; private set; }

        public double? RawObserved { get; private set; }

        public double BaselineRawValue { get; private set; }

        public double? RawDelta { get; private set; }

        public ComparisonStatus Status { get; private set; }

        public List<ComparisonReason> Reasons { get; private set; } = new();

        public string Detail { get; private set; } = default!;

        public string Fingerprint { get; private set; } = default!;

        public int CalculationVersion { get; private set; }

        public bool IsCurrent { get; private set; }

        public DateTimeOffset CalculatedAt { get; private set; }

        public static BaselineParameterFlight Record(long id, Baseline baseline, BaselineParameter parameter, ObservedMeasurement observed, BaselineComparison comparison, string fingerprint, DateTimeOffset now)
            => new(id, baseline, parameter, observed, comparison, fingerprint, now);

        public ComparisonDetail ReadDetail() => ComparisonDetail.FromJson(Detail);

        public void Withdraw()
        {
            IsCurrent = false;
        }

        public ComparisonStatus StatusFor(FlightState flightState, long? currentAnalysisRevisionId)
        {
            if (flightState != FlightState.ACCEPTED)
                return ComparisonStatus.NOT_FINAL;

            if (!IsCurrent || currentAnalysisRevisionId != AnalysisRevisionId)
                return ComparisonStatus.STALE_REVISION;

            return Status;
        }
    }
}
