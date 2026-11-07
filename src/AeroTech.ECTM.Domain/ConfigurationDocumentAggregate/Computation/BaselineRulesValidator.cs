namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class BaselineRulesValidator
    {
        public const string ActiveCatalogRequired = "ACTIVE_CATALOG_REQUIRED";

        private const string ConditionsSection = "conditions";

        public static BaselineRulesValidation Validate(string content, ValidatedCatalog? catalog)
        {
            var issues = new List<ValidationIssue>();
            var rules = ContentJson.Read<BaselineRulesContent>(content, issues);

            if (rules is null)
                return new BaselineRulesValidation(null, issues);

            var check = new RuleDocumentCheck(catalog, issues);

            Header(rules, check);
            Conditions(rules.Conditions, check);
            RuleDecisions.Check(rules.Decisions, check, target => TargetExists(rules, target));

            return new BaselineRulesValidation(issues.Count == 0 ? rules : null, issues);
        }

        private static void Header(BaselineRulesContent rules, RuleDocumentCheck check)
        {
            check.Text(rules.RulesCode, "$.rulesCode");
            check.Text(rules.Title, "$.title");
            check.Text(rules.Description, "$.description");
            check.Text(rules.Source, "$.source");
            check.Required(rules.CalibrationStatus, "$.calibrationStatus");
            check.Required(rules.EngineeringSignOff, "$.engineeringSignOff");
        }

        private static void Conditions(IReadOnlyList<BaselineConditionRule>? conditions, RuleDocumentCheck check)
        {
            check.Entries(conditions, "$.conditions", allowEmpty: true);

            var codes = new HashSet<string>(StringComparer.Ordinal);

            for (var item = 0; item < (conditions?.Count ?? 0); item++)
            {
                var path = $"$.conditions[{item}]";

                if (conditions![item] is not { } condition)
                {
                    check.Required(null, path);
                    continue;
                }

                if (condition.PerEngine is true)
                    check.EngineParameter(condition.Parameter, $"{path}.parameter");
                else
                    check.NumericChannel(condition.Parameter, $"{path}.parameter");

                check.Required(condition.PerEngine, $"{path}.perEngine");
                check.Positive(condition.Tolerance, $"{path}.tolerance");

                if (condition.Parameter is not null && !codes.Add(condition.Parameter))
                    check.Issue($"{path}.parameter", RuleDocumentCheck.CodeDuplicate, $"{condition.Parameter} is defined twice.");
            }
        }

        private static bool TargetExists(BaselineRulesContent rules, string? target)
        {
            if (target is null)
                return false;

            if (target == ConditionsSection)
                return true;

            var parts = target.Split('/', 2);

            return parts.Length == 2
                   && parts[0] == ConditionsSection
                   && (rules.Conditions ?? []).Any(condition => condition?.Parameter == parts[1]);
        }
    }
}
