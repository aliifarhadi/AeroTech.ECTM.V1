using System.Text.Json;
using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed record QualityResult(
        int SampleCount,
        IReadOnlyList<int> ElapsedGaps,
        bool? PartialStart,
        bool? PartialEnd,
        int? AirGroundLagSec,
        int? FirstRadioHeightAirborneIndex,
        int? FirstAirFlagIndex,
        IReadOnlyList<Discontinuity> Discontinuities,
        IReadOnlyList<QualitySpan> Spans,
        ClockEvidence? Clock,
        IReadOnlyList<ChannelCoverage> Coverage,
        IReadOnlyList<ValidationFinding> Findings,
        QualityGrade Grade,
        IReadOnlyList<string> GradeReasons)
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public static QualityResult FromJson(string json) => JsonSerializer.Deserialize<QualityResult>(json, Options)!;

        public string ToJson() => JsonSerializer.Serialize(this, Options);
    }
}
