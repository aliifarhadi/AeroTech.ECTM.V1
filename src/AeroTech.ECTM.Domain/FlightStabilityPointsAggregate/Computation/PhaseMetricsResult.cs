using System.Text.Json;
using System.Text.Json.Serialization;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record PhaseMetricsResult(IReadOnlyList<PhaseMetricGroup> Groups, ControlContextResult? ControlContext)
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static PhaseMetricsResult FromJson(string json) => JsonSerializer.Deserialize<PhaseMetricsResult>(json, Options)!;

        public string ToJson() => JsonSerializer.Serialize(this, Options);
    }
}
