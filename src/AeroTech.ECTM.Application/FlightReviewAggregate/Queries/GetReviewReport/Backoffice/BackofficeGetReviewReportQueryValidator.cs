using FluentValidation;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Queries.GetReviewReport.Backoffice
{
    public sealed class BackofficeGetReviewReportQueryValidator : AbstractValidator<BackofficeGetReviewReportQuery>
    {
        public BackofficeGetReviewReportQueryValidator()
        {
            RuleFor(query => query.To).GreaterThanOrEqualTo(query => query.From);
        }
    }
}
