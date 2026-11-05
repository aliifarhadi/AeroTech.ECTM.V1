using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record ParsedRecording(
        bool SignatureMatches,
        int ExpectedColumnCount,
        int HeaderColumnCount,
        IReadOnlyList<HeaderMismatch> HeaderMismatches,
        int SampleCount,
        RecordedChannel? TimeChannel,
        IReadOnlyList<RecordedChannel> Channels,
        IReadOnlyList<RecordingRow> FirstRows,
        IReadOnlyList<RecordingRow> LastRows,
        IReadOnlyList<int> EnginePositions,
        IReadOnlyList<ValidationFinding> Findings)
    {
        public double? FirstElapsed => TimeChannel?.Numbers!.FirstOrDefault(value => value is not null);

        public double? LastElapsed => TimeChannel?.Numbers!.LastOrDefault(value => value is not null);

        public int CountOf(ValidationSeverity severity) => Findings.Count(finding => finding.Severity == severity);

        public IReadOnlyList<string> DistinctTextsOf(string parameterCode)
        {
            var channel = Channels.FirstOrDefault(candidate => candidate.ParameterCode == parameterCode && candidate.Position is null);

            if (channel is null)
                return [];

            var texts = channel.Texts ?? channel.Numbers!.Select(number => number?.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();

            return texts
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Select(text => text!.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }
    }
}
