using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class QualityRulesValidator
    {
        public const string FieldRequired = "FIELD_REQUIRED";
        public const string ValueInvalid = "VALUE_INVALID";
        public const string ChannelUnknown = "CHANNEL_UNKNOWN";
        public const string ChannelKindInvalid = "CHANNEL_KIND_INVALID";
        public const string SpanCodeUnknown = "SPAN_CODE_UNKNOWN";
        public const string ActiveCatalogRequired = "ACTIVE_CATALOG_REQUIRED";

        public static readonly IReadOnlySet<string> CalibrationStatuses = new HashSet<string>(StringComparer.Ordinal)
        {
            "CANDIDATE",
            "CALIBRATED"
        };

        public static QualityRulesValidation Validate(string content, ValidatedCatalog? catalog)
        {
            var issues = new List<ValidationIssue>();
            var rules = ContentJson.Read<QualityRulesContent>(content, issues);

            if (rules is null)
                return new QualityRulesValidation(null, issues);

            var check = new Checker(catalog, issues);

            check.Text(rules.RulesCode, "$.rulesCode");
            check.OneOf(rules.CalibrationStatus, CalibrationStatuses, "$.calibrationStatus");
            check.Channels(rules.Channels);
            check.AirGroundLiterals(rules.AirGroundLiterals);
            check.AltitudeDiscontinuity(rules.AltitudeDiscontinuity);
            check.AirGroundLag(rules.AirGroundLag);
            check.EngineLag(rules.EngineLag);
            check.Partial(rules.Partial);
            check.Clock(rules.Clock);
            check.Coverage(rules.Coverage);
            check.Plausibility(rules.Plausibility);
            check.StuckSensor(rules.StuckSensor);
            check.Grade(rules.Grade, rules.Coverage);
            check.Affects(rules.Affects);

            return new QualityRulesValidation(issues.Count == 0 ? rules : null, issues);
        }

        private sealed class Checker(ValidatedCatalog? catalog, List<ValidationIssue> issues)
        {
            public void Channels(QualityChannels? channels)
            {
                if (Section(channels, "$.channels") is not { } present)
                    return;

                NumericChannel(present.Altitude, "$.channels.altitude", needsPositions: false);
                NumericChannel(present.RadioHeight, "$.channels.radioHeight", needsPositions: false);
                NumericChannel(present.Groundspeed, "$.channels.groundspeed", needsPositions: false);
                NumericChannel(present.EngineN1, "$.channels.engineN1", needsPositions: true);
                TextChannel(present.AirGround, "$.channels.airGround");
                TextChannel(present.PrimaryClock, "$.channels.primaryClock");
                TextChannel(present.SecondaryClock, "$.channels.secondaryClock");
            }

            public void AirGroundLiterals(AirGroundLiterals? literals)
            {
                if (Section(literals, "$.airGroundLiterals") is not { } present)
                    return;

                Text(present.Air, "$.airGroundLiterals.air");
                Text(present.Ground, "$.airGroundLiterals.ground");

                if (present.Air is not null && present.Air == present.Ground)
                    issues.Add(new ValidationIssue("$.airGroundLiterals", ValueInvalid, "The AIR and GROUND literals must differ."));
            }

            public void AltitudeDiscontinuity(AltitudeDiscontinuityRules? rules)
            {
                if (Section(rules, "$.altitudeDiscontinuity") is not { } present)
                    return;

                Positive(present.AltitudeFtPerSec, "$.altitudeDiscontinuity.altitudeFtPerSec");
                Positive(present.RadioHeightFtPerSec, "$.altitudeDiscontinuity.radioHeightFtPerSec");
                NotNegative(present.UntrustedBeforeSec, "$.altitudeDiscontinuity.untrustedBeforeSec");
                NotNegative(present.UntrustedAfterSec, "$.altitudeDiscontinuity.untrustedAfterSec");
            }

            public void AirGroundLag(AirGroundLagRules? rules)
            {
                if (Section(rules, "$.airGroundLag") is not { } present)
                    return;

                NotNegative(present.RadioHeightAboveFt, "$.airGroundLag.radioHeightAboveFt");
                Positive(present.MinSustainedSec, "$.airGroundLag.minSustainedSec");
                NotNegative(present.AirborneRadioHeightFt, "$.airGroundLag.airborneRadioHeightFt");
            }

            public void EngineLag(EngineLagRules? rules)
            {
                if (Section(rules, "$.engineLag") is not { } present)
                    return;

                Percent(present.IdleN1BelowPct, "$.engineLag.idleN1BelowPct");
                NotNegative(present.RadioHeightAboveFt, "$.engineLag.radioHeightAboveFt");
                NotNegative(present.RadioHeightRiseFt, "$.engineLag.radioHeightRiseFt");

                if (present.RiseLookbackSamples is not >= 1)
                    issues.Add(new ValidationIssue("$.engineLag.riseLookbackSamples", ValueInvalid, "The look-back must be at least 1 sample."));
            }

            public void Partial(PartialRules? rules)
            {
                if (Section(rules, "$.partial") is not { } present)
                    return;

                NotNegative(present.StartRadioHeightAboveFt, "$.partial.startRadioHeightAboveFt");
                NotNegative(present.EndGroundspeedAtLeastKt, "$.partial.endGroundspeedAtLeastKt");
            }

            public void Clock(ClockRules? rules)
            {
                if (Section(rules, "$.clock") is not { } present)
                    return;

                NotNegative(present.StepToleranceSec, "$.clock.stepToleranceSec");

                if (present.AcceptOneDigitHour is null)
                    issues.Add(new ValidationIssue("$.clock.acceptOneDigitHour", FieldRequired, "Say whether a one-digit hour (H:MM:SS) is a valid clock value."));
            }

            public void Coverage(CoverageRules? rules)
            {
                if (Section(rules, "$.coverage") is not { } present)
                    return;

                Percent(present.WarningBelowPct, "$.coverage.warningBelowPct");
                Percent(present.BlockingBelowPct, "$.coverage.blockingBelowPct");

                if (present.BlockingBelowPct > present.WarningBelowPct)
                    issues.Add(new ValidationIssue("$.coverage.blockingBelowPct", ValueInvalid, "The blocking level must not be above the warning level."));

                if (present.RequiredChannels is null || present.RequiredChannels.Count == 0)
                {
                    issues.Add(new ValidationIssue("$.coverage.requiredChannels", FieldRequired, "At least one required channel is needed."));
                    return;
                }

                for (var index = 0; index < present.RequiredChannels.Count; index++)
                    RecordedKey(present.RequiredChannels[index], $"$.coverage.requiredChannels[{index}]");
            }

            public void Plausibility(PlausibilityRules? rules)
            {
                if (Section(rules, "$.plausibility") is not { } present)
                    return;

                Percent(present.WarningAbovePct, "$.plausibility.warningAbovePct");
            }

            public void StuckSensor(StuckSensorRules? rules)
            {
                if (Section(rules, "$.stuckSensor") is not { } present)
                    return;

                Percent(present.LiveN1AtLeastPct, "$.stuckSensor.liveN1AtLeastPct");

                if (present.Channels is null)
                {
                    issues.Add(new ValidationIssue("$.stuckSensor.channels", FieldRequired, "List the stuck-sensor channels; use [] for none."));
                    return;
                }

                for (var index = 0; index < present.Channels.Count; index++)
                {
                    var path = $"$.stuckSensor.channels[{index}]";

                    NumericChannel(present.Channels[index].ParameterCode, $"{path}.parameterCode", needsPositions: true);
                    Positive(present.Channels[index].MinConstantSec, $"{path}.minConstantSec");

                    if (present.Channels[index].MinN1ChangePct is { } change && change is not (> 0 and <= 100))
                        issues.Add(new ValidationIssue($"{path}.minN1ChangePct", ValueInvalid, "The N1 change must be a percentage above 0."));
                }
            }

            public void Grade(GradeRules? rules, CoverageRules? coverage)
            {
                if (Section(rules, "$.grade") is not { } present)
                    return;

                Percent(present.AMinCoveragePct, "$.grade.aMinCoveragePct");

                if (present.AMinCoveragePct < coverage?.WarningBelowPct)
                    issues.Add(new ValidationIssue("$.grade.aMinCoveragePct", ValueInvalid, "Grade A cannot accept less coverage than the warning level."));

                var allowed = present.MasksAllowedForA ?? [];

                for (var index = 0; index < allowed.Count; index++)
                {
                    var path = $"$.grade.masksAllowedForA[{index}]";

                    SpanCode(allowed[index].Code ?? string.Empty, $"{path}.code");

                    if (allowed[index].Cause is { } cause && !QualitySpanCodes.DiscontinuityCauses.Contains(cause))
                        issues.Add(new ValidationIssue($"{path}.cause", ValueInvalid, $"'{cause}' is not a discontinuity cause."));
                }
            }

            public void Affects(IReadOnlyDictionary<string, IReadOnlyList<string>>? affects)
            {
                if (affects is null)
                {
                    issues.Add(new ValidationIssue("$.affects", FieldRequired, "List the downstream uses each span affects."));
                    return;
                }

                foreach (var (code, uses) in affects)
                {
                    SpanCode(code, $"$.affects.{code}");

                    if (uses is null || uses.Count == 0 || uses.Any(string.IsNullOrWhiteSpace))
                        issues.Add(new ValidationIssue($"$.affects.{code}", ValueInvalid, "Each span lists at least one downstream use."));
                }
            }

            public void Text(string? value, string path)
            {
                if (string.IsNullOrWhiteSpace(value))
                    issues.Add(new ValidationIssue(path, FieldRequired, "A value is required."));
            }

            public void OneOf(string? value, IReadOnlySet<string> allowed, string path)
            {
                if (value is null || !allowed.Contains(value))
                    issues.Add(new ValidationIssue(path, ValueInvalid, $"'{value}' is not one of {string.Join(", ", allowed)}."));
            }

            private TSection? Section<TSection>(TSection? section, string path)
                where TSection : class
            {
                if (section is null)
                    issues.Add(new ValidationIssue(path, FieldRequired, "This section is required."));

                return section;
            }

            private void Positive(double? value, string path)
            {
                if (value is not > 0)
                    issues.Add(new ValidationIssue(path, ValueInvalid, "A positive number is required."));
            }

            private void NotNegative(double? value, string path)
            {
                if (value is not >= 0)
                    issues.Add(new ValidationIssue(path, ValueInvalid, "A number of zero or more is required."));
            }

            private void Percent(double? value, string path)
            {
                if (value is not (>= 0 and <= 100))
                    issues.Add(new ValidationIssue(path, ValueInvalid, "A percentage from 0 to 100 is required."));
            }

            private void SpanCode(string code, string path)
            {
                if (!QualitySpanCodes.All.Contains(code))
                    issues.Add(new ValidationIssue(path, SpanCodeUnknown, $"'{code}' is not a quality span code."));
            }

            private void NumericChannel(string? code, string path, bool needsPositions)
            {
                if (EntryOf(code, path) is not { } entry)
                    return;

                if (!entry.IsNumeric || entry.IsDerived)
                    issues.Add(new ValidationIssue(path, ChannelKindInvalid, $"{code} must be a recorded numeric parameter."));

                if (needsPositions && entry.Positions.Count == 0)
                    issues.Add(new ValidationIssue(path, ChannelKindInvalid, $"{code} must be an engine parameter with positions."));
            }

            private void TextChannel(string? code, string path)
            {
                if (EntryOf(code, path) is not { } entry)
                    return;

                if (entry.IsNumeric)
                    issues.Add(new ValidationIssue(path, ChannelKindInvalid, $"{code} must be a text or enumerated parameter."));
            }

            private void RecordedKey(string key, string path)
            {
                var reference = ParameterReference.Parse(key);

                if (reference is null || reference.UsesPositionPlaceholder)
                {
                    issues.Add(new ValidationIssue(path, ValueInvalid, $"'{key}' must be CODE or CODE[1|2]."));
                    return;
                }

                if (EntryOf(reference.Code, path) is not { } entry)
                    return;

                var positionMatches = reference.Position is { } position ? entry.Positions.Contains(position) : entry.Positions.Count == 0;

                if (!positionMatches)
                    issues.Add(new ValidationIssue(path, ValueInvalid, $"'{key}' does not match the positions of {reference.Code}."));
            }

            private CatalogEntry? EntryOf(string? code, string path)
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    issues.Add(new ValidationIssue(path, FieldRequired, "A parameter code is required."));
                    return null;
                }

                if (catalog is null)
                    return null;

                var entry = catalog.Find(code);

                if (entry is null)
                    issues.Add(new ValidationIssue(path, ChannelUnknown, $"{code} is not in the active parameter catalog."));

                return entry;
            }
        }
    }
}
