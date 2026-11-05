using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Dto
{
    public sealed record BackofficeReviewReportDto(
        DateOnly From,
        DateOnly To,
        IReadOnlyList<BackofficeReviewerCountDto> ByReviewer,
        IReadOnlyList<BackofficeReviewReportEntryDto> Entries,
        IReadOnlyList<BackofficePublicationDto> Publications);

    public sealed record BackofficeReviewerCountDto(string? Reviewer, EnumValueDto Action, int Count);

    public sealed record BackofficeReviewReportEntryDto(long FlightId, long AnalysisRevisionId, BackofficeReviewEntryDto Entry);

    public sealed record BackofficePublicationDto(long DocumentId, EnumValueDto Kind, int VersionNumber, string? PublishedBy, DateTimeOffset? PublishedUtc, string? ApprovalNote);
}
