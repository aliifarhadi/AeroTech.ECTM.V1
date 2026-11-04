using System.Text.Json;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.CreateConfigurationDraft
{
    public interface ICreateConfigurationDraftCommand
    {
        string Kind { get; }

        JsonElement? Content { get; }

        long? BasedOnDocumentId { get; }
    }
}
