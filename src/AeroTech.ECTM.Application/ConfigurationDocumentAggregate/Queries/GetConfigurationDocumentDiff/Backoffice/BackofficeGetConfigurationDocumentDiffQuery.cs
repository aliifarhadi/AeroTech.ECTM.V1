using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentDiff.Backoffice
{
    public sealed record BackofficeGetConfigurationDocumentDiffQuery(string Kind, long FromDocumentId, long ToDocumentId) : IRequest<BackofficeConfigurationDiffDto>;
}
