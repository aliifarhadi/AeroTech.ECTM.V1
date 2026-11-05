using System.Globalization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public static class QualityEvaluator
    {
        public const string TimeAxisNotIncreasing = "TIME_AXIS_NOT_STRICTLY_INCREASING";
        public const string InputMissing = "QUALITY_INPUT_MISSING";
        public const string RequiredChannelCoverage = "REQUIRED_CHANNEL_COVERAGE";
        public const string PlausibilityShareHigh = "PLAUSIBILITY_SHARE_HIGH";

        private const string Seconds = "s";
        private const string FeetPerSecond = "ft/s";

        public static QualityResult Evaluate(QualityRulesContent rules, QualityInput input)
        {
            var coverage = CoverageOf(rules.Coverage!, input);
            var findings = new List<ValidationFinding>();

            AddCoverageFindings(rules.Coverage!, coverage, findings);
            AddPlausibilityFindings(rules.Plausibility!, input, findings);

            var timeline = QualityTimeline.Of(input);

            if (timeline is null)
                return NotEvaluable(input, coverage, findings);

            AddMissingInputFindings(rules, timeline, findings);

            var discontinuities = new List<Discontinuity>();
            var spans = new List<QualitySpan>();

            AddDiscontinuities(rules, timeline, discontinuities, spans);
            AddAirGroundLagSpans(rules, timeline, spans);
            AddEngineLagSpans(rules, timeline, spans);
            AddStuckSensorSpans(rules, timeline, spans);

            var orderedSpans = spans
                .OrderBy(span => span.StartIndex)
                .ThenBy(span => span.Code, StringComparer.Ordinal)
                .ToList();
            var radioHeightAirborne = FirstRadioHeightAirborne(rules, timeline);
            var airFlag = FirstAirFlag(rules, timeline);
            var airGroundLag = LagBetween(timeline, radioHeightAirborne, airFlag);
            var partialStart = PartialStart(rules, timeline);
            var partialEnd = PartialEnd(rules, timeline);
            var clock = ClockOf(rules, timeline);
            var grade = QualityGrader.Grade(rules, input.HasBlockingValidation, coverage, findings, orderedSpans, timeline.GapStarts, partialStart, partialEnd);

            return new QualityResult(
                input.SampleCount,
                timeline.GapStarts,
                partialStart,
                partialEnd,
                airGroundLag,
                radioHeightAirborne,
                airFlag,
                discontinuities,
                orderedSpans,
                clock,
                coverage,
                findings,
                grade.Grade,
                grade.Reasons);
        }

        private static QualityResult NotEvaluable(QualityInput input, IReadOnlyList<ChannelCoverage> coverage, List<ValidationFinding> findings)
        {
            var message = "The time axis is missing or not strictly increasing, so no quality rule can be evaluated.";
            findings.Add(new ValidationFinding(ValidationSeverity.BLOCKING, TimeAxisNotIncreasing, message, null, null, null));

            return new QualityResult(input.SampleCount, [], null, null, null, null, null, [], [], null, coverage, findings, QualityGrade.NOT_USABLE, [TimeAxisNotIncreasing]);
        }

        private static void AddDiscontinuities(QualityRulesContent rules, QualityTimeline timeline, List<Discontinuity> discontinuities, List<QualitySpan> spans)
        {
            var thresholds = rules.AltitudeDiscontinuity!;
            var altitudeKey = rules.Channels!.Altitude!;
            var radioHeightKey = rules.Channels.RadioHeight!;
            var altitude = timeline.Numbers(altitudeKey);
            var radioHeight = timeline.Numbers(radioHeightKey);
            var t = timeline.Elapsed;

            for (var index = 1; index < timeline.Count; index++)
            {
                if (timeline.StartsAfterGap(index))
                    continue;

                var seconds = t[index] - t[index - 1];
                var altitudeRate = RateOf(altitude, index, seconds);
                var radioHeightRate = RateOf(radioHeight, index, seconds);
                var altitudeHit = altitudeRate is { } a && Math.Abs(a) > thresholds.AltitudeFtPerSec;
                var radioHeightHit = radioHeightRate is { } r && Math.Abs(r) > thresholds.RadioHeightFtPerSec;

                if (!altitudeHit && !radioHeightHit)
                    continue;

                var sameSign = altitudeHit && radioHeightHit && (altitudeRate > 0) == (radioHeightRate > 0);
                var code = sameSign ? QualitySpanCodes.DiscontinuityConfirmed : QualitySpanCodes.DiscontinuitySingleChannel;
                var (start, end) = UntrustedWindow(timeline, index, thresholds);

                discontinuities.Add(new Discontinuity(index, altitudeRate, radioHeightRate, radioHeight[index - 1], code));
                spans.Add(SpanOf(rules, timeline, QualitySpanCodes.AltitudeRateUntrusted, start, end, [altitudeKey, radioHeightKey])
                    with { JumpIndex = index, Cause = code, Actual = altitudeRate ?? radioHeightRate, Threshold = thresholds.AltitudeFtPerSec, Unit = FeetPerSecond });
            }
        }

        private static (int Start, int End) UntrustedWindow(QualityTimeline timeline, int jumpIndex, AltitudeDiscontinuityRules thresholds)
        {
            var t = timeline.Elapsed;
            var block = timeline.Block;
            var start = jumpIndex;
            var end = jumpIndex;

            while (start > 0 && block[start - 1] == block[jumpIndex] && t[jumpIndex] - t[start - 1] <= thresholds.UntrustedBeforeSec)
                start--;

            while (end + 1 < timeline.Count && block[end + 1] == block[jumpIndex] && t[end + 1] - t[jumpIndex] <= thresholds.UntrustedAfterSec)
                end++;

            return (start, end);
        }

        private static void AddAirGroundLagSpans(QualityRulesContent rules, QualityTimeline timeline, List<QualitySpan> spans)
        {
            var lagRules = rules.AirGroundLag!;
            var airGroundKey = rules.Channels!.AirGround!;
            var radioHeightKey = rules.Channels.RadioHeight!;
            var airGround = timeline.Texts(airGroundKey);
            var radioHeight = timeline.Numbers(radioHeightKey);
            var ground = rules.AirGroundLiterals!.Ground;

            var groundWhileHigh = Enumerable.Range(0, timeline.Count)
                .Select(index => airGround[index] == ground && radioHeight[index] > lagRules.RadioHeightAboveFt)
                .ToList();

            foreach (var (start, end) in timeline.RunsOf(groundWhileHigh))
            {
                var duration = timeline.Elapsed[end] - timeline.Elapsed[start] + 1;

                if (duration < lagRules.MinSustainedSec)
                    continue;

                spans.Add(SpanOf(rules, timeline, QualitySpanCodes.AirGroundLagsRadioHeight, start, end, [airGroundKey, radioHeightKey])
                    with { Actual = duration, Threshold = lagRules.MinSustainedSec, Unit = Seconds });
            }
        }

        private static void AddEngineLagSpans(QualityRulesContent rules, QualityTimeline timeline, List<QualitySpan> spans)
        {
            var lagRules = rules.EngineLag!;
            var radioHeightKey = rules.Channels!.RadioHeight!;
            var engineKeys = timeline.PositionKeysOf(rules.Channels.EngineN1!);

            if (engineKeys.Count == 0)
                return;

            var radioHeight = timeline.Numbers(radioHeightKey);
            var engines = engineKeys.Select(timeline.Numbers).ToList();
            var lookback = lagRules.RiseLookbackSamples!.Value;
            var flags = new List<bool>();

            for (var index = 0; index < timeline.Count; index++)
            {
                var rising = index >= lookback
                             && radioHeight[index] is { } now
                             && radioHeight[index - lookback] is { } before
                             && timeline.Block[index] == timeline.Block[index - lookback]
                             && now - before > lagRules.RadioHeightRiseFt;
                var idle = engines.All(n1 => n1[index] is { } value && value < lagRules.IdleN1BelowPct);

                flags.Add(rising && idle && radioHeight[index] > lagRules.RadioHeightAboveFt);
            }

            foreach (var (start, end) in timeline.RunsOf(flags))
            {
                var duration = timeline.Elapsed[end] - timeline.Elapsed[start] + 1;

                spans.Add(SpanOf(rules, timeline, QualitySpanCodes.EngineChannelsLagKinematics, start, end, [.. engineKeys, radioHeightKey])
                    with { Actual = duration, Unit = Seconds });
            }
        }

        private static void AddStuckSensorSpans(QualityRulesContent rules, QualityTimeline timeline, List<QualitySpan> spans)
        {
            var stuckRules = rules.StuckSensor!;
            var engineN1 = rules.Channels!.EngineN1!;

            foreach (var channelRule in stuckRules.Channels!)
            {
                foreach (var key in timeline.PositionKeysOf(channelRule.ParameterCode!))
                {
                    var position = ParameterReference.Parse(key)!.Position;
                    var values = timeline.Numbers(key);
                    var n1 = timeline.Numbers($"{engineN1}[{position}]");

                    foreach (var (start, end) in ConstantRuns(timeline, values, n1, stuckRules.LiveN1AtLeastPct!.Value))
                    {
                        var duration = timeline.Elapsed[end] - timeline.Elapsed[start] + 1;

                        var n1Change = RangeOf(n1, start, end);

                        if (duration < channelRule.MinConstantSec)
                            continue;

                        if (channelRule.MinN1ChangePct is { } minimumChange && n1Change < minimumChange)
                            continue;

                        spans.Add(SpanOf(rules, timeline, QualitySpanCodes.SensorStuck, start, end, [key])
                            with { Actual = duration, Threshold = channelRule.MinConstantSec, Unit = Seconds, Note = $"The value stayed {Format(values[start]!.Value)} while N1 moved {Format(n1Change)} %." });
                    }
                }
            }
        }

        private static List<(int Start, int End)> ConstantRuns(QualityTimeline timeline, IReadOnlyList<double?> values, IReadOnlyList<double?> n1, double liveN1AtLeast)
        {
            var runs = new List<(int Start, int End)>();
            int? start = null;

            for (var index = 0; index <= timeline.Count; index++)
            {
                var continues = index < timeline.Count
                                && start is { } first
                                && !timeline.StartsAfterGap(index)
                                && IsLive(values, n1, index, liveN1AtLeast)
                                && values[index] == values[first];

                if (continues)
                    continue;

                if (start is { } runStart)
                    runs.Add((runStart, index - 1));

                start = index < timeline.Count && IsLive(values, n1, index, liveN1AtLeast) ? index : null;
            }

            return runs;
        }

        private static double RangeOf(IReadOnlyList<double?> values, int start, int end)
        {
            var window = Enumerable.Range(start, end - start + 1).Select(index => values[index]!.Value).ToList();

            return window.Max() - window.Min();
        }

        private static bool IsLive(IReadOnlyList<double?> values, IReadOnlyList<double?> n1, int index, double liveN1AtLeast)
            => values[index] is not null && n1[index] is { } speed && speed >= liveN1AtLeast;

        private static int? FirstRadioHeightAirborne(QualityRulesContent rules, QualityTimeline timeline)
        {
            var radioHeight = timeline.Numbers(rules.Channels!.RadioHeight!);
            var airborne = rules.AirGroundLag!.AirborneRadioHeightFt;

            for (var index = 0; index < timeline.Count && timeline.Block[index] == 0; index++)
            {
                if (radioHeight[index] > airborne)
                    return index;
            }

            return null;
        }

        private static int? FirstAirFlag(QualityRulesContent rules, QualityTimeline timeline)
        {
            var airGround = timeline.Texts(rules.Channels!.AirGround!);
            var air = rules.AirGroundLiterals!.Air;

            for (var index = 0; index < timeline.Count && timeline.Block[index] == 0; index++)
            {
                if (airGround[index] == air)
                    return index;
            }

            return null;
        }

        private static int? LagBetween(QualityTimeline timeline, int? radioHeightAirborne, int? airFlag)
        {
            if (radioHeightAirborne is not { } first || airFlag is not { } flag)
                return null;

            return (int)Math.Round(timeline.Elapsed[flag] - timeline.Elapsed[first]);
        }

        private static bool? PartialStart(QualityRulesContent rules, QualityTimeline timeline)
        {
            var radioHeightKey = rules.Channels!.RadioHeight!;

            if (!timeline.IsRecorded(radioHeightKey))
                return null;

            return timeline.Numbers(radioHeightKey)[0] > rules.Partial!.StartRadioHeightAboveFt;
        }

        private static bool? PartialEnd(QualityRulesContent rules, QualityTimeline timeline)
        {
            var groundspeedKey = rules.Channels!.Groundspeed!;

            if (!timeline.IsRecorded(groundspeedKey))
                return null;

            return timeline.Numbers(groundspeedKey)[timeline.Count - 1] >= rules.Partial!.EndGroundspeedAtLeastKt;
        }

        private static ClockEvidence ClockOf(QualityRulesContent rules, QualityTimeline timeline)
        {
            var primaryTexts = timeline.Texts(rules.Channels!.PrimaryClock!);
            var secondaryTexts = timeline.Texts(rules.Channels.SecondaryClock!);
            var tolerance = rules.Clock!.StepToleranceSec!.Value;
            var acceptOneDigitHour = rules.Clock.AcceptOneDigitHour!.Value;
            var offsets = new SortedDictionary<int, int>();
            var disagree = 0;
            var invalid = 0;
            var primaryNotInStep = 0;
            var secondaryNotInStep = 0;
            int? previousPrimary = null;
            int? previousSecondary = null;

            for (var index = 0; index < timeline.Count; index++)
            {
                var primary = ClockTime.SecondsOf(primaryTexts[index], acceptOneDigitHour);
                var secondary = ClockTime.SecondsOf(secondaryTexts[index], acceptOneDigitHour);

                if (primary is null || secondary is null)
                    invalid++;

                if (primary is { } p && secondary is { } s)
                {
                    var offset = p - s;
                    offsets[offset] = offsets.GetValueOrDefault(offset) + 1;

                    if (offset != 0)
                        disagree++;
                }

                if (index > 0)
                {
                    var elapsedStep = timeline.Elapsed[index] - timeline.Elapsed[index - 1];

                    if (NotInStep(primary, previousPrimary, elapsedStep, tolerance))
                        primaryNotInStep++;

                    if (NotInStep(secondary, previousSecondary, elapsedStep, tolerance))
                        secondaryNotInStep++;
                }

                previousPrimary = primary;
                previousSecondary = secondary;
            }

            var trusted = disagree == 0 && invalid == 0 && primaryNotInStep == 0 && secondaryNotInStep == 0;
            var histogram = offsets.ToDictionary(pair => pair.Key.ToString(CultureInfo.InvariantCulture), pair => pair.Value);

            return new ClockEvidence(disagree, invalid, primaryNotInStep, secondaryNotInStep, histogram, trusted ? TimeTrust.TRUSTED : TimeTrust.UNTRUSTED);
        }

        private static bool NotInStep(int? current, int? previous, double elapsedStep, double tolerance)
        {
            if (current is not { } now || previous is not { } before)
                return false;

            var clockStep = ClockTime.DayModulo(now - before);

            return Math.Abs(clockStep - ClockTime.DayModulo(elapsedStep)) > tolerance;
        }

        private static List<ChannelCoverage> CoverageOf(CoverageRules rules, QualityInput input)
        {
            var coverage = new List<ChannelCoverage>();

            foreach (var key in rules.RequiredChannels!)
            {
                var statistics = input.Channels.FirstOrDefault(channel => channel.ParameterKey == key);

                if (statistics is null || statistics.SampleCount == 0)
                {
                    coverage.Add(new ChannelCoverage(key, statistics is not null, input.SampleCount, 0, 0));
                    continue;
                }

                var percent = 100.0 * statistics.ValuedCount / statistics.SampleCount;
                coverage.Add(new ChannelCoverage(key, true, statistics.SampleCount, statistics.ValuedCount, percent));
            }

            return coverage;
        }

        private static void AddCoverageFindings(CoverageRules rules, IReadOnlyList<ChannelCoverage> coverage, List<ValidationFinding> findings)
        {
            foreach (var channel in coverage)
            {
                ValidationSeverity? severity = null;

                if (channel.Percent < rules.WarningBelowPct)
                    severity = ValidationSeverity.WARNING;

                if (channel.Percent < rules.BlockingBelowPct)
                    severity = ValidationSeverity.BLOCKING;

                if (severity is not { } level)
                    continue;

                var recorded = channel.Recorded ? "" : " (not recorded)";
                var message = $"{channel.ParameterKey}{recorded}: {Format(channel.Percent)} % of samples have a value; warning below {Format(rules.WarningBelowPct!.Value)} %, blocking below {Format(rules.BlockingBelowPct!.Value)} %.";

                findings.Add(new ValidationFinding(level, RequiredChannelCoverage, message, channel.ParameterKey, channel.SampleCount - channel.ValuedCount, channel.Percent));
            }
        }

        private static void AddPlausibilityFindings(PlausibilityRules rules, QualityInput input, List<ValidationFinding> findings)
        {
            foreach (var channel in input.Channels.Where(channel => channel.OutOfRangeCount > 0 && channel.ValuedCount > 0))
            {
                var share = 100.0 * channel.OutOfRangeCount / channel.ValuedCount;

                if (share <= rules.WarningAbovePct)
                    continue;

                var message = $"{channel.ParameterKey}: {Format(share)} % of valued samples are outside the catalog range; warning above {Format(rules.WarningAbovePct!.Value)} %.";

                findings.Add(new ValidationFinding(ValidationSeverity.WARNING, PlausibilityShareHigh, message, channel.ParameterKey, channel.OutOfRangeCount, share));
            }
        }

        private static void AddMissingInputFindings(QualityRulesContent rules, QualityTimeline timeline, List<ValidationFinding> findings)
        {
            var channels = rules.Channels!;
            string[] aircraftKeys = [channels.Altitude!, channels.RadioHeight!, channels.AirGround!, channels.Groundspeed!, channels.PrimaryClock!, channels.SecondaryClock!];
            var missing = aircraftKeys.Where(key => !timeline.IsRecorded(key)).ToList();

            if (timeline.PositionKeysOf(channels.EngineN1!).Count == 0)
                missing.Add(channels.EngineN1!);

            foreach (var key in missing)
            {
                var message = $"{key} is not recorded; the quality rules that need it give no result for it.";
                findings.Add(new ValidationFinding(ValidationSeverity.WARNING, InputMissing, message, key, null, null));
            }
        }

        private static QualitySpan SpanOf(QualityRulesContent rules, QualityTimeline timeline, string code, int start, int end, IReadOnlyList<string> keys)
            => new(
                code,
                start,
                end,
                keys,
                keys.Select(timeline.OrdinalOf).ToList(),
                null,
                null,
                null,
                null,
                null,
                rules.Affects!.GetValueOrDefault(code) ?? [],
                null);

        private static double? RateOf(IReadOnlyList<double?> values, int index, double seconds)
        {
            if (values[index] is not { } now || values[index - 1] is not { } before)
                return null;

            return (now - before) / seconds;
        }

        private static string Format(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
