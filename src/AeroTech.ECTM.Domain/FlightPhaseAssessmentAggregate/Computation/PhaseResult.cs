using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseResult(
        RuleDocumentHeader RuleSet,
        PhaseReferenceComparison Reference,
        int SampleCount,
        string? NotEvaluableReason,
        IReadOnlyList<AnchorResult> Anchors,
        IReadOnlyList<PhaseCoverage> Coverage,
        IReadOnlyList<PhaseSegment> Segments,
        IReadOnlyList<PhaseTransition> Transitions,
        IReadOnlyList<FlightEventSpan> Events,
        IReadOnlyList<EventChannelCoverage> EventCoverage)
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public bool IsEvaluated => NotEvaluableReason is null;

        public AnchorResult? AnchorOf(string code) => Anchors.FirstOrDefault(anchor => anchor.Code == code);

        public static PhaseResult FromJson(string json) => JsonSerializer.Deserialize<PhaseResult>(json, Options)!;

        public string ToJson() => JsonSerializer.Serialize(this, Options);
    }
}
