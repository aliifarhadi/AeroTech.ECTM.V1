namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class SnapshotRulesValidator
    {
        public const string ActiveCatalogRequired = "ACTIVE_CATALOG_REQUIRED";

        private static readonly IReadOnlySet<string> Sections = new HashSet<string>(StringComparer.Ordinal)
        {
            "channels",
            "engineParameters",
            "conditionParameters",
            "configurationChannels",
            "masks",
            "takeoff",
            "cruise",
            "paired",
            "phaseMetrics",
            "controlContext"
        };

        public static SnapshotRulesValidation Validate(string content, ValidatedCatalog? catalog)
        {
            var issues = new List<ValidationIssue>();
            var rules = ContentJson.Read<SnapshotRulesContent>(content, issues);

            if (rules is null)
                return new SnapshotRulesValidation(null, issues, new HashSet<string>());

            var check = new RuleDocumentCheck(catalog, issues);

            Header(rules, check);
            Channels(rules.Channels, check);
            EngineParameters(rules, check);
            ConditionParameters(rules.ConditionParameters, check);
            ConfigurationChannels(rules.ConfigurationChannels, check);
            Masks(rules.Masks, check);
            Takeoff(rules.Takeoff, check);
            Cruise(rules.Cruise, check);
            Paired(rules.Paired, check);
            PhaseMetrics(rules.PhaseMetrics, check);
            ControlContext(rules.ControlContext, check);
            RuleDecisions.Check(rules.Decisions, check, target => TargetExists(rules, target));

            return new SnapshotRulesValidation(issues.Count == 0 ? rules : null, issues, check.Parameters);
        }

        private static void Header(SnapshotRulesContent rules, RuleDocumentCheck check)
        {
            check.Text(rules.RulesCode, "$.rulesCode");
            check.Text(rules.Title, "$.title");
            check.Text(rules.Description, "$.description");
            check.Text(rules.Source, "$.source");
            check.Required(rules.CalibrationStatus, "$.calibrationStatus");
            check.Required(rules.EngineeringSignOff, "$.engineeringSignOff");
        }

        private static void Channels(SnapshotChannels? channels, RuleDocumentCheck check)
        {
            if (Section(channels, "$.channels", check) is not { } present)
                return;

            check.EngineParameter(present.EngineN1, "$.channels.engineN1");
            check.EngineParameter(present.EngineEgt, "$.channels.engineEgt");
            check.NumericChannel(present.Altitude, "$.channels.altitude");
            check.NumericChannel(present.Mach, "$.channels.mach");
            check.TextChannel(present.Toga, "$.channels.toga");
        }

        private static void EngineParameters(SnapshotRulesContent rules, RuleDocumentCheck check)
        {
            var parameters = rules.EngineParameters;

            check.Entries(parameters, "$.engineParameters", allowEmpty: false);

            for (var item = 0; item < (parameters?.Count ?? 0); item++)
                check.EngineParameter(parameters![item], $"$.engineParameters[{item}]");

            if (parameters is null || rules.Channels is not { } channels)
                return;

            foreach (var required in new[] { channels.EngineN1, channels.EngineEgt })
            {
                if (required is not null && !parameters.Contains(required))
                    check.Issue("$.engineParameters", RuleDocumentCheck.ValueInvalid, $"{required} is reported in every window and must be listed.");
            }
        }

        private static void ConditionParameters(IReadOnlyList<string>? parameters, RuleDocumentCheck check)
        {
            check.Entries(parameters, "$.conditionParameters", allowEmpty: true);

            for (var item = 0; item < (parameters?.Count ?? 0); item++)
                check.NumericChannel(parameters![item], $"$.conditionParameters[{item}]");
        }

        private static void ConfigurationChannels(IReadOnlyList<string>? channels, RuleDocumentCheck check)
        {
            check.Entries(channels, "$.configurationChannels", allowEmpty: true);

            for (var item = 0; item < (channels?.Count ?? 0); item++)
                check.TextChannel(channels![item], $"$.configurationChannels[{item}]");
        }

        private static void Masks(SnapshotMasks? masks, RuleDocumentCheck check)
        {
            if (Section(masks, "$.masks", check) is not { } present)
                return;

            MaskList(present.Untrusted, "$.masks.untrusted", check);
            MaskList(present.Coherence, "$.masks.coherence", check);
            check.Required(present.GapNeighboursUntrusted, "$.masks.gapNeighboursUntrusted");
        }

        private static void Takeoff(TakeoffRules? takeoff, RuleDocumentCheck check)
        {
            if (Section(takeoff, "$.takeoff", check) is not { } present)
                return;

            check.Text(present.LiftoffAnchor, "$.takeoff.liftoffAnchor");
            check.NotNegative(present.SearchBeforeLiftoffSec, "$.takeoff.searchBeforeLiftoffSec");
            check.Positive(present.SearchAfterLiftoffSec, "$.takeoff.searchAfterLiftoffSec");
            check.Percent(present.PartialStartN1Pct, "$.takeoff.partialStartN1Pct");
            check.NotNegative(present.WindowBeforePeakSec, "$.takeoff.windowBeforePeakSec");
            check.NotNegative(present.WindowAfterPeakSec, "$.takeoff.windowAfterPeakSec");
            check.AtLeast(present.MinValidN1Samples, 1, "$.takeoff.minValidN1Samples");
        }

        private static void Cruise(CruiseRules? cruise, RuleDocumentCheck check)
        {
            if (Section(cruise, "$.cruise", check) is not { } present)
                return;

            check.Required(present.Phase, "$.cruise.phase");
            check.Entries(present.Selectors, "$.cruise.selectors", allowEmpty: false);

            var codes = new HashSet<string>(StringComparer.Ordinal);

            for (var item = 0; item < (present.Selectors?.Count ?? 0); item++)
            {
                var path = $"$.cruise.selectors[{item}]";

                if (Section(present.Selectors![item], path, check) is not { } selector)
                    continue;

                Selector(selector, path, check);

                if (selector.Code is not null && !codes.Add(selector.Code))
                    check.Issue($"{path}.code", RuleDocumentCheck.CodeDuplicate, $"{selector.Code} is defined twice.");
            }
        }

        private static void Selector(CruiseSelector selector, string path, RuleDocumentCheck check)
        {
            check.Text(selector.Code, $"{path}.code");
            check.Positive(selector.N1TolerancePct, $"{path}.n1TolerancePct");
            check.NotNegative(selector.AltitudeRangeFt, $"{path}.altitudeRangeFt");
            check.NotNegative(selector.MachRange, $"{path}.machRange");
            check.AtLeast(selector.WindowSec, 1, $"{path}.windowSec");
            check.Positive(selector.MinBlockSec, $"{path}.minBlockSec");
            check.AtLeast(selector.ReportSec, 1, $"{path}.reportSec");

            if (selector.ReportSec > selector.MinBlockSec)
                check.Issue($"{path}.reportSec", RuleDocumentCheck.ValueInvalid, "The report window cannot be longer than the minimum block (docs/05 section 2.6).");
        }

        private static void Paired(PairedRules? paired, RuleDocumentCheck check)
        {
            if (Section(paired, "$.paired", check) is not { } present)
                return;

            check.EngineParameter(present.Parameter, "$.paired.parameter");
            check.AtLeast(present.MinMatchedSamples, 1, "$.paired.minMatchedSamples");
        }

        private static void PhaseMetrics(PhaseMetricRules? metrics, RuleDocumentCheck check)
        {
            if (Section(metrics, "$.phaseMetrics", check) is not { } present)
                return;

            check.Percent(present.Percentile, "$.phaseMetrics.percentile");
        }

        private static void ControlContext(ControlContextRules? control, RuleDocumentCheck check)
        {
            if (Section(control, "$.controlContext", check) is not { } present)
                return;

            check.Entries(present.Phases, "$.controlContext.phases", allowEmpty: false);
            check.Entries(present.EngineChannels, "$.controlContext.engineChannels", allowEmpty: false);
            check.Entries(present.ModeChannels, "$.controlContext.modeChannels", allowEmpty: true);
            check.Text(present.VerificationNote, "$.controlContext.verificationNote");

            for (var item = 0; item < (present.EngineChannels?.Count ?? 0); item++)
                check.EngineParameter(present.EngineChannels![item], $"$.controlContext.engineChannels[{item}]");

            for (var item = 0; item < (present.ModeChannels?.Count ?? 0); item++)
                check.TextChannel(present.ModeChannels![item], $"$.controlContext.modeChannels[{item}]");
        }

        private static void MaskList(IReadOnlyList<string>? masks, string path, RuleDocumentCheck check)
        {
            check.Entries(masks, path, allowEmpty: true);

            for (var item = 0; item < (masks?.Count ?? 0); item++)
                check.Mask(masks![item], $"{path}[{item}]");
        }

        private static bool TargetExists(SnapshotRulesContent rules, string? target)
        {
            if (target is null)
                return false;

            if (Sections.Contains(target))
                return true;

            var parts = target.Split('/', 2);

            return parts.Length == 2
                   && parts[0] == "cruise"
                   && (rules.Cruise?.Selectors ?? []).Any(selector => selector?.Code == parts[1]);
        }

        private static TSection? Section<TSection>(TSection? section, string path, RuleDocumentCheck check)
            where TSection : class
        {
            if (section is null)
                check.Required(null, path);

            return section;
        }
    }
}
