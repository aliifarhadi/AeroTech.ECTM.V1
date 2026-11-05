using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Queries.GetReviewReport
{
    public sealed class GetReviewReportService : IGetReviewReportService
    {
        private readonly IFlightReviewRepository _reviews;
        private readonly IConfigurationDocumentRepository _documents;

        public GetReviewReportService(IFlightReviewRepository reviews, IConfigurationDocumentRepository documents)
        {
            _reviews = reviews;
            _documents = documents;
        }

        public async Task<BackofficeReviewReportDto> ExecuteAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
        {
            var start = new DateTimeOffset(from.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            var end = new DateTimeOffset(to.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            var reviews = await _reviews.ListWithEntriesBetweenAsync(start, end, cancellationToken);
            var entries = reviews
                .SelectMany(review => review.Entries
                    .Where(entry => entry.At >= start && entry.At < end)
                    .Select(entry => new BackofficeReviewReportEntryDto(review.FlightId, review.AnalysisRevisionId, BackofficeReviewEntryDto.From(entry))))
                .OrderBy(entry => entry.Entry.AtUtc)
                .ToList();
            var byReviewer = entries
                .GroupBy(entry => (entry.Entry.Actor, entry.Entry.Action.Value))
                .OrderBy(group => group.Key.Actor, StringComparer.Ordinal)
                .ThenBy(group => group.Key.Value)
                .Select(group => new BackofficeReviewerCountDto(group.Key.Actor, group.First().Entry.Action, group.Count()))
                .ToList();
            var publications = await _documents.ListPublishedBetweenAsync(start, end, cancellationToken);

            return new BackofficeReviewReportDto(
                from,
                to,
                byReviewer,
                entries,
                publications.Select(document => new BackofficePublicationDto(document.Id, EnumValueDto.Of(document.Kind), document.VersionNumber, document.PublishedBy, document.PublishedAt, document.ApprovalNote)).ToList());
        }
    }
}
