using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class PhaseRuleSetValidator
    {
        public const string FieldRequired = "FIELD_REQUIRED";
        public const string ValueInvalid = "VALUE_INVALID";
        public const string ChannelUnknown = "CHANNEL_UNKNOWN";
        public const string ChannelKindInvalid = "CHANNEL_KIND_INVALID";
        public const string ReferenceUnknown = "REFERENCE_UNKNOWN";
        public const string CodeDuplicate = "CODE_DUPLICATE";
        public const string ActiveCatalogRequired = "ACTIVE_CATALOG_REQUIRED";

        public const string DocumentTarget = "document";

        public static PhaseRuleSetValidation Validate(string content, ValidatedCatalog? catalog)
        {
            var issues = new List<ValidationIssue>();
            var rules = ContentJson.Read<PhaseRuleSetContent>(content, issues);

            if (rules is null)
                return new PhaseRuleSetValidation(null, issues, new HashSet<string>());

            var check = new PhaseRuleSetCheck(catalog, issues);
            var targets = new DecisionTargets();

            Header(rules, check);
            DerivedChannels(rules.DerivedChannels, check);
            Anchors(rules.Anchors, check);
            check.ConditionsAvailable = true;
            Conditions(rules.Conditions, check);
            LabelRules(rules.LabelRules, check, targets);
            Fallback(rules.Fallback, check);
            PostProcessing(rules.PostProcessing, check, targets);
            Segments(rules.Segments, check);
            Events(rules.Events, check, targets);
            Decisions(rules.Decisions, check, targets);

            return new PhaseRuleSetValidation(issues.Count == 0 ? rules : null, issues, check.Parameters);
        }

        private static void Header(PhaseRuleSetContent rules, PhaseRuleSetCheck check)
        {
            check.Text(rules.RuleSetCode, "$.ruleSetCode");
            check.Text(rules.Title, "$.title");
            check.Text(rules.Description, "$.description");
            check.Text(rules.Source, "$.source");
            check.Required(rules.CalibrationStatus, "$.calibrationStatus");
            check.Required(rules.EngineeringSignOff, "$.engineeringSignOff");
        }

        private static void DerivedChannels(IReadOnlyList<DerivedChannel>? channels, PhaseRuleSetCheck check)
        {
            check.Entries(channels, "$.derivedChannels", allowEmpty: true);

            for (var item = 0; item < (channels?.Count ?? 0); item++)
            {
                var path = $"$.derivedChannels[{item}]";

                if (Present(channels![item], path, check) is not { } channel)
                    continue;

                channel.Check(check, path);
                check.DefineDerivedChannel(channel.Code, $"{path}.code");
            }
        }

        private static void Anchors(IReadOnlyList<PhaseAnchor>? anchors, PhaseRuleSetCheck check)
        {
            check.Entries(anchors, "$.anchors", allowEmpty: true);

            for (var item = 0; item < (anchors?.Count ?? 0); item++)
            {
                var path = $"$.anchors[{item}]";

                if (Present(anchors![item], path, check) is not { } anchor)
                    continue;

                anchor.Check(check, path);
                check.DefineAnchor(anchor.Code, $"{path}.code");
            }
        }

        private static void Conditions(IReadOnlyList<PhaseCondition>? conditions, PhaseRuleSetCheck check)
        {
            check.Entries(conditions, "$.conditions", allowEmpty: true);

            for (var item = 0; item < (conditions?.Count ?? 0); item++)
            {
                var path = $"$.conditions[{item}]";

                if (Present(conditions![item], path, check) is not { } condition)
                    continue;

                condition.Check(check, path);
                check.DefineCondition(condition.Code, $"{path}.code");
            }
        }

        private static void LabelRules(IReadOnlyList<LabelRule>? rules, PhaseRuleSetCheck check, DecisionTargets targets)
        {
            check.Entries(rules, "$.labelRules", allowEmpty: false);

            for (var item = 0; item < (rules?.Count ?? 0); item++)
            {
                var path = $"$.labelRules[{item}]";

                if (Present(rules![item], path, check) is not { } rule)
                    continue;

                rule.Check(check, path);

                if (rule.Id is not null && !targets.LabelRules.Add(rule.Id))
                    check.Issue($"{path}.id", CodeDuplicate, $"{rule.Id} is defined twice.");
            }
        }

        private static void Fallback(FallbackLabel? fallback, PhaseRuleSetCheck check)
        {
            if (fallback is null)
            {
                check.Required(null, "$.fallback");
                return;
            }

            check.Label(fallback.Label, fallback.UnknownReason, "$.fallback");
        }

        private static void PostProcessing(IReadOnlyList<PhasePostProcessor>? steps, PhaseRuleSetCheck check, DecisionTargets targets)
        {
            check.Entries(steps, "$.postProcessing", allowEmpty: true);

            for (var item = 0; item < (steps?.Count ?? 0); item++)
            {
                var path = $"$.postProcessing[{item}]";

                if (Present(steps![item], path, check) is not { } step)
                    continue;

                step.Check(check, path);
                targets.PostProcessing.Add(step.Name);
            }
        }

        private static void Segments(SegmentRules? segments, PhaseRuleSetCheck check)
        {
            if (segments is null)
            {
                check.Required(null, "$.segments");
                return;
            }

            segments.Check(check, "$.segments");
        }

        private static void Events(IReadOnlyList<EventDefinition>? events, PhaseRuleSetCheck check, DecisionTargets targets)
        {
            check.Entries(events, "$.events", allowEmpty: true);

            for (var item = 0; item < (events?.Count ?? 0); item++)
            {
                var path = $"$.events[{item}]";

                if (Present(events![item], path, check) is not { } definition)
                    continue;

                definition.Check(check, path);

                if (definition.Code is not null && !targets.Events.Add(definition.Code))
                    check.Issue($"{path}.code", CodeDuplicate, $"{definition.Code} is defined twice.");
            }
        }

        private static void Decisions(IReadOnlyList<RuleDecision>? decisions, PhaseRuleSetCheck check, DecisionTargets targets)
        {
            check.Entries(decisions, "$.decisions", allowEmpty: true);

            var ids = new HashSet<string>(StringComparer.Ordinal);

            for (var item = 0; item < (decisions?.Count ?? 0); item++)
            {
                var path = $"$.decisions[{item}]";

                if (Present(decisions![item], path, check) is not { } decision)
                    continue;

                check.Text(decision.Id, $"{path}.id");
                check.Text(decision.Topic, $"{path}.topic");
                check.Text(decision.Choice, $"{path}.choice");
                check.Text(decision.Basis, $"{path}.basis");
                check.Text(decision.Alternative, $"{path}.alternative");
                check.Text(decision.HowToChange, $"{path}.howToChange");

                if (decision.Id is not null && !ids.Add(decision.Id))
                    check.Issue($"{path}.id", CodeDuplicate, $"{decision.Id} is defined twice.");

                check.Entries(decision.AppliesTo, $"{path}.appliesTo", allowEmpty: false);

                for (var target = 0; target < (decision.AppliesTo?.Count ?? 0); target++)
                {
                    if (!targets.Exists(decision.AppliesTo![target], check))
                        check.Issue($"{path}.appliesTo[{target}]", ReferenceUnknown, $"'{decision.AppliesTo[target]}' does not name a part of this document.");
                }
            }
        }

        private static TItem? Present<TItem>(TItem? item, string path, PhaseRuleSetCheck check)
            where TItem : class
        {
            if (item is null)
                check.Required(null, path);

            return item;
        }

        private sealed class DecisionTargets
        {
            public HashSet<string> LabelRules { get; } = new(StringComparer.Ordinal);

            public HashSet<string> PostProcessing { get; } = new(StringComparer.Ordinal);

            public HashSet<string> Events { get; } = new(StringComparer.Ordinal);

            public bool Exists(string? target, PhaseRuleSetCheck check)
            {
                if (target == DocumentTarget || target == "fallback" || target == "segments")
                    return true;

                var parts = target?.Split('/', 2) ?? [];

                if (parts.Length != 2)
                    return false;

                return parts[0] switch
                {
                    "derivedChannels" => check.DerivedChannels.Contains(parts[1]),
                    "anchors" => check.Anchors.Contains(parts[1]),
                    "conditions" => check.Conditions.Contains(parts[1]),
                    "labelRules" => LabelRules.Contains(parts[1]),
                    "postProcessing" => PostProcessing.Contains(parts[1]),
                    "events" => Events.Contains(parts[1]),
                    _ => false
                };
            }
        }
    }
}
