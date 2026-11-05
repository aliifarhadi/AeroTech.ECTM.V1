using System.Text.Json;
using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record StabilityPointsResult(
        RuleDocumentHeader RuleSet,
        int SampleCount,
        SnapshotReason? NotEvaluableReason,
        IReadOnlyList<int> Positions,
        IReadOnlyList<TakeoffN1PeakContext> TakeoffN1Peaks,
        IReadOnlyList<TakeoffEgtPeakObservation> TakeoffEgtPeaks,
        IReadOnlyList<CruiseSelectorResult> Cruise)
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static StabilityPointsResult FromJson(string json) => JsonSerializer.Deserialize<StabilityPointsResult>(json, Options)!;

        public string ToJson() => JsonSerializer.Serialize(this, Options);
    }
}
