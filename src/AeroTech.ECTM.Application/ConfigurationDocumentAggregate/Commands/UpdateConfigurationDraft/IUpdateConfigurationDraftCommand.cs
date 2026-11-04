using System.Text.Json;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.UpdateConfigurationDraft
{
    public interface IUpdateConfigurationDraftCommand
    {
        string Kind { get; }

        long DocumentId { get; }

        JsonElement Content { get; }
    }
}
