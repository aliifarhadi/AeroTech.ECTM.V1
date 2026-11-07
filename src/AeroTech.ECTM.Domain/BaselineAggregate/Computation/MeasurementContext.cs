using System.Text.Json;
using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Domain.BaselineAggregate.Computation
{
    public sealed record MeasurementContext(
        IReadOnlyList<ConditionValue> Conditions,
        IReadOnlyList<ConditionValue> EngineValues,
        string? Configuration)
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static readonly MeasurementContext Unknown = new([], [], null);

        public bool IsUnknown => Conditions.Count == 0 && Configuration is null;

        public bool IsEmpty => IsUnknown && EngineValues.Count == 0;

        public static MeasurementContext Of(SnapshotReading reading)
        {
            var conditions = new List<ConditionValue>();
            var engineValues = new List<ConditionValue>();

            foreach (var condition in reading.Conditions)
            {
                if (condition.Mean is { } mean)
                    conditions.Add(new ConditionValue(condition.ParameterKey, mean));
            }

            foreach (var value in reading.Values)
            {
                var reference = ParameterReference.Parse(value.ParameterKey);

                if (reference is null || reference.Position != reading.Position)
                    continue;

                if (value.Mean is { } mean)
                    engineValues.Add(new ConditionValue(reference.Code, mean));
            }

            return new MeasurementContext(conditions, engineValues, reading.SingleConfiguration);
        }

        public static MeasurementContext FromJson(string? json)
            => json is null ? Unknown : JsonSerializer.Deserialize<MeasurementContext>(json, Options)!;

        public string? ToJson() => IsEmpty ? null : JsonSerializer.Serialize(this, Options);

        public double? ConditionOf(string parameterCode)
            => Conditions.FirstOrDefault(condition => condition.ParameterCode == parameterCode)?.Value;

        public double? EngineValueOf(string parameterCode)
            => EngineValues.FirstOrDefault(value => value.ParameterCode == parameterCode)?.Value;
    }
}
