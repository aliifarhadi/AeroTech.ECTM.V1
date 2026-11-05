using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightReviewAggregate;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Dto
{
    public sealed record BackofficeFlightReviewDto(
        long Id,
        long FlightId,
        long AnalysisRevisionId,
        EnumValueDto State,
        string? DecidedBy,
        DateTimeOffset? DecidedUtc,
        EnumValueDto? ReasonCode,
        string? Note,
        IReadOnlyList<BackofficeReviewEntryDto> Entries)
    {
        public static BackofficeFlightReviewDto From(FlightReview review)
            => new(
                review.Id,
                review.FlightId,
                review.AnalysisRevisionId,
                EnumValueDto.Of(review.State),
                review.DecidedBy,
                review.DecidedAt,
                EnumValueDto.OfNullable(review.ReasonCode),
                review.Note,
                review.Entries.OrderBy(entry => entry.At).Select(BackofficeReviewEntryDto.From).ToList());
    }
}
