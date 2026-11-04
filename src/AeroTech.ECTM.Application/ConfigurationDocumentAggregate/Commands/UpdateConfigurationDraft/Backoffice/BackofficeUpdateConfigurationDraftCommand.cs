using System.Text.Json;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.UpdateConfigurationDraft.Backoffice
{
    public sealed record BackofficeUpdateConfigurationDraftCommand(
        string Kind,
        long DocumentId,
        JsonElement Content) : IRequest<BackofficeConfigurationDocumentDetailDto>, IUpdateConfigurationDraftCommand;
}
