using FluentValidation;

namespace AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated.Backoffice
{
    public sealed class BackofficeGetAuditEntriesPaginatedQueryValidator : AbstractValidator<BackofficeGetAuditEntriesPaginatedQuery>
    {
        public BackofficeGetAuditEntriesPaginatedQueryValidator()
        {
            RuleFor(query => query.To)
                .GreaterThanOrEqualTo(query => query.From)
                .When(query => query.From is not null && query.To is not null);
        }
    }
}
