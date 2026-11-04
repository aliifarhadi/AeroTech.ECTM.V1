using System.Text.Json;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.CreateConfigurationDraft.Backoffice
{
    public sealed record BackofficeCreateConfigurationDraftCommand(
        string Kind,
        JsonElement? Content,
        long? BasedOnDocumentId) : IRequest<BackofficeConfigurationDocumentDetailDto>, ICreateConfigurationDraftCommand;
}
