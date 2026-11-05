using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed record QualityInput(
        int SampleCount,
        IReadOnlyList<double?> Elapsed,
        IReadOnlyDictionary<string, IReadOnlyList<double?>> Numbers,
        IReadOnlyDictionary<string, IReadOnlyList<string?>> Texts,
        IReadOnlyList<ChannelStatistics> Channels,
        bool HasBlockingValidation)
    {
        public static QualityInput From(ParsedRecording recording)
        {
            var numbers = recording.Channels
                .Where(channel => channel.IsNumeric)
                .ToDictionary(channel => channel.ParameterKey, channel => channel.Numbers!);
            var texts = recording.Channels
                .Where(channel => !channel.IsNumeric)
                .ToDictionary(channel => channel.ParameterKey, channel => channel.Texts!);
            var statistics = recording.Channels
                .Select(channel => new ChannelStatistics(channel.ParameterKey, channel.ParameterCode, channel.Ordinal, channel.SampleCount, channel.ValuedCount, channel.OutOfRangeCount))
                .ToList();
            var hasBlocking = recording.Findings.Any(finding => finding.Severity == ValidationSeverity.BLOCKING);

            return new QualityInput(recording.SampleCount, recording.TimeChannel?.Numbers ?? [], numbers, texts, statistics, hasBlocking);
        }
    }
}
