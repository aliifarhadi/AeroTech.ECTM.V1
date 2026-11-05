using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using AeroTech.ECTM.Domain.FlightReviewAggregate;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto
{
    public sealed record BackofficeAnalysisRevisionDto(
        long Id,
        long FlightId,
        int Number,
        EnumValueDto Trigger,
        EnumValueDto Status,
        string SourceSha256,
        long DataProfileDocumentId,
        int DataProfileVersion,
        long ParameterCatalogDocumentId,
        int ParameterCatalogVersion,
        long QualityRulesDocumentId,
        int QualityRulesVersion,
        long? PhaseRuleSetDocumentId,
        int? PhaseRuleSetVersion,
        long? SnapshotRulesDocumentId,
        int? SnapshotRulesVersion,
        string EngineAssignments,
        long? ProcessingJobId,
        string? FailureCode,
        DateTimeOffset CreatedUtc,
        DateTimeOffset? CompletedUtc,
        DateTimeOffset? SupersededUtc,
        BackofficeFlightReviewDto? Review)
    {
        public static BackofficeAnalysisRevisionDto From(AnalysisRevision revision, FlightReview? review = null)
            => new(
                revision.Id,
                revision.FlightId,
                revision.Number,
                EnumValueDto.Of(revision.Trigger),
                EnumValueDto.Of(revision.Status),
                revision.SourceSha256,
                revision.DataProfileDocumentId,
                revision.DataProfileVersion,
                revision.ParameterCatalogDocumentId,
                revision.ParameterCatalogVersion,
                revision.QualityRulesDocumentId,
                revision.QualityRulesVersion,
                revision.PhaseRuleSetDocumentId,
                revision.PhaseRuleSetVersion,
                revision.SnapshotRulesDocumentId,
                revision.SnapshotRulesVersion,
                revision.EngineAssignments,
                revision.ProcessingJobId,
                revision.FailureCode,
                revision.CreatedAt,
                revision.CompletedAt,
                revision.SupersededAt,
                review is null ? null : BackofficeFlightReviewDto.From(review));
    }
}
