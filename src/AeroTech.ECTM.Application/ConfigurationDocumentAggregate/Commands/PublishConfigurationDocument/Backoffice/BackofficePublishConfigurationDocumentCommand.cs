using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.PublishConfigurationDocument.Backoffice
{
    public sealed record BackofficePublishConfigurationDocumentCommand(
        string Kind,
        long DocumentId,
        string ApprovalNote) : IRequest<BackofficeConfigurationDocumentDetailDto>, IPublishConfigurationDocumentCommand;
}
