using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.RetireConfigurationDocument.Backoffice
{
    public sealed record BackofficeRetireConfigurationDocumentCommand(
        string Kind,
        long DocumentId,
        string? Note) : IRequest<BackofficeConfigurationDocumentDetailDto>, IRetireConfigurationDocumentCommand;
}
