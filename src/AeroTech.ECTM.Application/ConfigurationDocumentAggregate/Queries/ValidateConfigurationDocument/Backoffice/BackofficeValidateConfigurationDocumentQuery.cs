using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.ValidateConfigurationDocument.Backoffice
{
    public sealed record BackofficeValidateConfigurationDocumentQuery(string Kind, long DocumentId) : IRequest<BackofficeValidationResultDto>;
}
