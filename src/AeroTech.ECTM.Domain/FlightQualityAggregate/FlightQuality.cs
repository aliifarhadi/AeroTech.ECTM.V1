using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightQualityAggregate
{
    public sealed class FlightQuality : AggregateRoot<long>
    {
        private FlightQuality()
        {
        }

        private FlightQuality(long id, long flightId, long analysisRevisionId, long qualityRulesDocumentId, int qualityRulesVersion, QualityResult result, DateTimeOffset evaluatedAt)
        {
            Id = id;
            FlightId = flightId;
            AnalysisRevisionId = analysisRevisionId;
            QualityRulesDocumentId = qualityRulesDocumentId;
            QualityRulesVersion = qualityRulesVersion;
            Status = ResultStatus.PROVISIONAL;
            Grade = result.Grade;
            TimeTrust = result.Clock?.TimeTrust;
            PartialStart = result.PartialStart;
            PartialEnd = result.PartialEnd;
            DiscontinuityCount = result.Discontinuities.Count;
            SpanCount = result.Spans.Count;
            EvaluatedAt = evaluatedAt;
            Result = result.ToJson();
        }

        public long FlightId { get; private set; }

        public long AnalysisRevisionId { get; private set; }

        public long QualityRulesDocumentId { get; private set; }

        public int QualityRulesVersion { get; private set; }

        public ResultStatus Status { get; private set; }

        public QualityGrade Grade { get; private set; }

        public TimeTrust? TimeTrust { get; private set; }

        public bool? PartialStart { get; private set; }

        public bool? PartialEnd { get; private set; }

        public int DiscontinuityCount { get; private set; }

        public int SpanCount { get; private set; }

        public DateTimeOffset EvaluatedAt { get; private set; }

        public string Result { get; private set; } = default!;

        public static FlightQuality Record(long id, long flightId, long analysisRevisionId, long qualityRulesDocumentId, int qualityRulesVersion, QualityResult result, DateTimeOffset now)
            => new(id, flightId, analysisRevisionId, qualityRulesDocumentId, qualityRulesVersion, result, now);

        public QualityResult ReadResult() => QualityResult.FromJson(Result);

        public void Supersede()
        {
            Status = ResultStatus.SUPERSEDED;
        }
    }
}
