using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.AnalysisRevisionAggregate
{
    public sealed class AnalysisRevision : AggregateRoot<long>
    {
        private AnalysisRevision()
        {
        }

        private AnalysisRevision(long id, int number, RevisionTrigger trigger, AnalysisRevisionArgs args, DateTimeOffset createdAt)
        {
            Id = id;
            FlightId = args.FlightId;
            Number = number;
            Trigger = trigger;
            Status = RevisionStatus.PENDING;
            SourceSha256 = args.SourceSha256;
            DataProfileDocumentId = args.Parsing.DataProfileDocumentId;
            DataProfileVersion = args.Parsing.DataProfileVersion;
            ParameterCatalogDocumentId = args.Parsing.ParameterCatalogDocumentId;
            ParameterCatalogVersion = args.Parsing.ParameterCatalogVersion;
            QualityRulesDocumentId = args.QualityRulesDocumentId;
            QualityRulesVersion = args.QualityRulesVersion;
            PhaseRuleSetDocumentId = args.PhaseRuleSetDocumentId;
            PhaseRuleSetVersion = args.PhaseRuleSetVersion;
            SnapshotRulesDocumentId = args.SnapshotRulesDocumentId;
            SnapshotRulesVersion = args.SnapshotRulesVersion;
            EngineAssignments = args.EngineAssignments;
            CreatedAt = createdAt;
        }

        public long FlightId { get; private set; }

        public int Number { get; private set; }

        public RevisionTrigger Trigger { get; private set; }

        public RevisionStatus Status { get; private set; }

        public string SourceSha256 { get; private set; } = default!;

        public long DataProfileDocumentId { get; private set; }

        public int DataProfileVersion { get; private set; }

        public long ParameterCatalogDocumentId { get; private set; }

        public int ParameterCatalogVersion { get; private set; }

        public long QualityRulesDocumentId { get; private set; }

        public int QualityRulesVersion { get; private set; }

        public long? PhaseRuleSetDocumentId { get; private set; }

        public int? PhaseRuleSetVersion { get; private set; }

        public long? SnapshotRulesDocumentId { get; private set; }

        public int? SnapshotRulesVersion { get; private set; }

        public string EngineAssignments { get; private set; } = default!;

        public long? ProcessingJobId { get; private set; }

        public string? FailureCode { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? CompletedAt { get; private set; }

        public DateTimeOffset? SupersededAt { get; private set; }

        public static AnalysisRevision Open(long id, int number, RevisionTrigger trigger, AnalysisRevisionArgs args, DateTimeOffset now)
            => new(id, number, trigger, args, now);

        public bool UsesInputs(long qualityRulesDocumentId, long? phaseRuleSetDocumentId, long? snapshotRulesDocumentId, string engineAssignments)
        {
            var sameRules = QualityRulesDocumentId == qualityRulesDocumentId
                            && PhaseRuleSetDocumentId == phaseRuleSetDocumentId
                            && SnapshotRulesDocumentId == snapshotRulesDocumentId;

            return sameRules && EngineAssignments == engineAssignments;
        }

        public void AssignJob(long processingJobId)
        {
            ProcessingJobId = processingJobId;
        }

        public void Activate(DateTimeOffset now)
        {
            Status = RevisionStatus.CURRENT;
            CompletedAt = now;
        }

        public void Supersede(DateTimeOffset now)
        {
            Status = RevisionStatus.SUPERSEDED;
            SupersededAt = now;
        }

        public void Fail(string failureCode, DateTimeOffset now)
        {
            Status = RevisionStatus.FAILED;
            FailureCode = failureCode;
            CompletedAt = now;
        }
    }
}
