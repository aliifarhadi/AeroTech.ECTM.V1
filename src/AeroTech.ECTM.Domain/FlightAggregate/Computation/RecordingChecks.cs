using System.Globalization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class RecordingChecks
    {
        public static IReadOnlyList<ValidationFinding> Of(RecordedChannel? timeChannel, IReadOnlyList<RecordedChannel> channels, int raggedRowCount, int expectedColumnCount, ValidatedCatalog catalog)
        {
            var findings = new List<ValidationFinding>();

            if (timeChannel is not null)
                AddTimeAxisFindings(timeChannel, findings);

            if (raggedRowCount > 0)
                findings.Add(new ValidationFinding(ValidationSeverity.WARNING, ValidationCodes.RaggedRows, $"{raggedRowCount} rows do not have {expectedColumnCount} cells.", null, raggedRowCount, null));

            foreach (var channel in channels)
            {
                AddCoverage(channel, findings);
                AddPlausibility(channel, catalog, findings);
                AddLiteralsOutsideDomain(channel, findings);
            }

            return findings;
        }

        private static void AddTimeAxisFindings(RecordedChannel timeChannel, List<ValidationFinding> findings)
        {
            var axis = TimeAxis.Of(timeChannel.Numbers!);
            var key = timeChannel.ParameterKey;

            if (axis.MissingCount > 0)
                findings.Add(new ValidationFinding(ValidationSeverity.BLOCKING, ValidationCodes.TimeAxisValueMissing, $"{axis.MissingCount} samples have no {key} value.", key, axis.MissingCount, null));

            if (axis.DuplicateCount > 0)
                findings.Add(new ValidationFinding(ValidationSeverity.BLOCKING, ValidationCodes.TimeAxisDuplicate, $"{key} repeats at {axis.DuplicateCount} samples.", key, axis.DuplicateCount, null));

            if (axis.DecreasingCount > 0)
                findings.Add(new ValidationFinding(ValidationSeverity.BLOCKING, ValidationCodes.TimeAxisDecreasing, $"{key} decreases at {axis.DecreasingCount} samples.", key, axis.DecreasingCount, null));

            if (axis.Gaps.Count > 0)
            {
                var message = $"{axis.Gaps.Count} gaps in {key} with {Format(axis.MissingSeconds)} s missing at the nominal step of {Format(axis.NominalStep!.Value)} s.";

                findings.Add(new ValidationFinding(ValidationSeverity.WARNING, ValidationCodes.TimeAxisGap, message, key, axis.Gaps.Count, axis.MissingSeconds));
            }
        }

        private static void AddCoverage(RecordedChannel channel, List<ValidationFinding> findings)
        {
            var withoutValue = channel.MissingCount + channel.UnparseableCount;

            if (withoutValue == 0)
                return;

            var coverage = channel.SampleCount == 0 ? 0 : 100.0 * channel.ValuedCount / channel.SampleCount;
            var message = $"{channel.ParameterKey}: {channel.ValuedCount} of {channel.SampleCount} samples have a value ({Format(coverage)} %); {channel.MissingCount} missing, {channel.UnparseableCount} unparseable.";

            findings.Add(new ValidationFinding(ValidationSeverity.INFO, ValidationCodes.ChannelCoverage, message, channel.ParameterKey, withoutValue, coverage));
        }

        private static void AddPlausibility(RecordedChannel channel, ValidatedCatalog catalog, List<ValidationFinding> findings)
        {
            if (channel.OutOfRangeCount == 0)
                return;

            var range = catalog.Find(channel.ParameterCode)!.Plausibility!;
            var share = 100.0 * channel.OutOfRangeCount / channel.ValuedCount;
            var message = $"{channel.ParameterKey}: {channel.OutOfRangeCount} values ({Format(share)} % of valued samples) are outside the catalog range [{Format(range.Min!.Value)}, {Format(range.Max!.Value)}] {channel.Unit}.";

            findings.Add(new ValidationFinding(ValidationSeverity.INFO, ValidationCodes.PlausibilityOutOfRange, message, channel.ParameterKey, channel.OutOfRangeCount, share));
        }

        private static void AddLiteralsOutsideDomain(RecordedChannel channel, List<ValidationFinding> findings)
        {
            if (channel.LiteralsOutsideDomain.Count == 0)
                return;

            var literals = channel.LiteralsOutsideDomain
                .OrderByDescending(pair => pair.Value)
                .ThenBy(pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => $"'{pair.Key}' ({pair.Value})");
            var count = channel.LiteralsOutsideDomain.Values.Sum();
            var message = $"{channel.ParameterKey}: literals outside the catalog's enumerated values: {string.Join(", ", literals)}.";

            findings.Add(new ValidationFinding(ValidationSeverity.WARNING, ValidationCodes.EnumLiteralOutsideDomain, message, channel.ParameterKey, count, null));
        }

        private static string Format(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
