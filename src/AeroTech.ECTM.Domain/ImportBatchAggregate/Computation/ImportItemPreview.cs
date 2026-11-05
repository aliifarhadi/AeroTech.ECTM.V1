using System.Text.Json;
using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Domain.ImportBatchAggregate.Computation
{
    public sealed record ImportItemPreview(
        IdentityResolution? Identity,
        int SampleCount,
        double? FirstElapsed,
        double? LastElapsed,
        IReadOnlyList<ValidationFinding> Findings,
        IReadOnlyList<HeaderMismatch> HeaderMismatches,
        string? RejectionMessage)
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public static ImportItemPreview Rejected(string message, IReadOnlyList<HeaderMismatch> headerMismatches)
            => new(null, 0, null, null, [], headerMismatches, message);

        public static ImportItemPreview FromJson(string json) => JsonSerializer.Deserialize<ImportItemPreview>(json, Options)!;

        public string ToJson() => JsonSerializer.Serialize(this, Options);
    }
}
