using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById.Backoffice
{
    public sealed record BackofficeGetConfigurationDocumentByIdQuery(string Kind, long DocumentId) : IRequest<BackofficeConfigurationDocumentDetailDto>;
}
