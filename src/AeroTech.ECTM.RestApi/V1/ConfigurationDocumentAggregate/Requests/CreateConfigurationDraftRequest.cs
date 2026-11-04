using System.Text.Json;

namespace AeroTech.ECTM.RestApi.V1.ConfigurationDocumentAggregate.Requests
{
    public sealed record CreateConfigurationDraftRequest(JsonElement? Content, long? BasedOnDocumentId);
}
