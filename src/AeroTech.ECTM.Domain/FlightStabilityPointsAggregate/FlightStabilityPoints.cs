using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate
{
    public sealed class FlightStabilityPoints : AggregateRoot<long>
    {
        private FlightStabilityPoints()
        {
        }

        private FlightStabilityPoints(long id, long flightId, long analysisRevisionId, long snapshotRulesDocumentId, int snapshotRulesVersion, StabilityPointsEvaluation evaluation, DateTimeOffset evaluatedAt)
        {
            Id = id;
            FlightId = flightId;
            AnalysisRevisionId = analysisRevisionId;
            SnapshotRulesDocumentId = snapshotRulesDocumentId;
            SnapshotRulesVersion = snapshotRulesVersion;
            Status = ResultStatus.PROVISIONAL;
            NotEvaluableReason = evaluation.Points.NotEvaluableReason;
            EvaluatedAt = evaluatedAt;
            Result = evaluation.Points.ToJson();
            PhaseMetrics = evaluation.Metrics.ToJson();
        }

        public long FlightId { get; private set; }

        public long AnalysisRevisionId { get; private set; }

        public long SnapshotRulesDocumentId { get; private set; }

        public int SnapshotRulesVersion { get; private set; }

        public ResultStatus Status { get; private set; }

        public SnapshotReason? NotEvaluableReason { get; private set; }

        public DateTimeOffset EvaluatedAt { get; private set; }

        public string Result { get; private set; } = default!;

        public string PhaseMetrics { get; private set; } = default!;

        public static FlightStabilityPoints Record(long id, long flightId, long analysisRevisionId, long snapshotRulesDocumentId, int snapshotRulesVersion, StabilityPointsEvaluation evaluation, DateTimeOffset now)
            => new(id, flightId, analysisRevisionId, snapshotRulesDocumentId, snapshotRulesVersion, evaluation, now);

        public StabilityPointsResult ReadResult() => StabilityPointsResult.FromJson(Result);

        public PhaseMetricsResult ReadPhaseMetrics() => PhaseMetricsResult.FromJson(PhaseMetrics);

        public void MarkFinal()
        {
            Status = ResultStatus.FINAL;
        }

        public void MarkProvisional()
        {
            Status = ResultStatus.PROVISIONAL;
        }

        public void Supersede()
        {
            Status = ResultStatus.SUPERSEDED;
        }
    }
}
