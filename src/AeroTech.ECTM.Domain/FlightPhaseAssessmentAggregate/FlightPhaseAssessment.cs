using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate
{
    public sealed class FlightPhaseAssessment : AggregateRoot<long>
    {
        private FlightPhaseAssessment()
        {
        }

        private FlightPhaseAssessment(long id, long flightId, long analysisRevisionId, long phaseRuleSetDocumentId, int phaseRuleSetVersion, PhaseResult result, DateTimeOffset evaluatedAt)
        {
            Id = id;
            FlightId = flightId;
            AnalysisRevisionId = analysisRevisionId;
            PhaseRuleSetDocumentId = phaseRuleSetDocumentId;
            PhaseRuleSetVersion = phaseRuleSetVersion;
            Status = ResultStatus.PROVISIONAL;
            ReferenceAlgorithm = result.Reference.Algorithm;
            ReferenceMatch = result.Reference.Match;
            NotEvaluableReason = result.NotEvaluableReason;
            SegmentCount = result.Segments.Count;
            UnknownSampleCount = result.Coverage.Where(entry => entry.PhaseCode == FlightPhaseCode.UNKNOWN).Sum(entry => entry.Samples);
            EventSpanCount = result.Events.Count;
            EvaluatedAt = evaluatedAt;
            Result = result.ToJson();
        }

        public long FlightId { get; private set; }

        public long AnalysisRevisionId { get; private set; }

        public long PhaseRuleSetDocumentId { get; private set; }

        public int PhaseRuleSetVersion { get; private set; }

        public ResultStatus Status { get; private set; }

        public string ReferenceAlgorithm { get; private set; } = default!;

        public ReferenceMatch ReferenceMatch { get; private set; }

        public string? NotEvaluableReason { get; private set; }

        public int SegmentCount { get; private set; }

        public int UnknownSampleCount { get; private set; }

        public int EventSpanCount { get; private set; }

        public DateTimeOffset EvaluatedAt { get; private set; }

        public string Result { get; private set; } = default!;

        public static FlightPhaseAssessment Record(long id, long flightId, long analysisRevisionId, long phaseRuleSetDocumentId, int phaseRuleSetVersion, PhaseResult result, DateTimeOffset now)
            => new(id, flightId, analysisRevisionId, phaseRuleSetDocumentId, phaseRuleSetVersion, result, now);

        public PhaseResult ReadResult() => PhaseResult.FromJson(Result);

        public void Supersede()
        {
            Status = ResultStatus.SUPERSEDED;
        }
    }
}
