namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class RuleDecisions
    {
        public const string DocumentTarget = "document";

        public static void Check(IReadOnlyList<RuleDecision>? decisions, RuleDocumentCheck check, Func<string?, bool> targetExists)
        {
            check.Entries(decisions, "$.decisions", allowEmpty: true);

            var ids = new HashSet<string>(StringComparer.Ordinal);

            for (var item = 0; item < (decisions?.Count ?? 0); item++)
            {
                var path = $"$.decisions[{item}]";

                if (decisions![item] is not { } decision)
                {
                    check.Required(null, path);
                    continue;
                }

                check.Text(decision.Id, $"{path}.id");
                check.Text(decision.Topic, $"{path}.topic");
                check.Text(decision.Choice, $"{path}.choice");
                check.Text(decision.Basis, $"{path}.basis");
                check.Text(decision.Alternative, $"{path}.alternative");
                check.Text(decision.HowToChange, $"{path}.howToChange");

                if (decision.Id is not null && !ids.Add(decision.Id))
                    check.Issue($"{path}.id", RuleDocumentCheck.CodeDuplicate, $"{decision.Id} is defined twice.");

                check.Entries(decision.AppliesTo, $"{path}.appliesTo", allowEmpty: false);

                for (var target = 0; target < (decision.AppliesTo?.Count ?? 0); target++)
                {
                    var name = decision.AppliesTo![target];

                    if (name != DocumentTarget && !targetExists(name))
                        check.Issue($"{path}.appliesTo[{target}]", RuleDocumentCheck.ReferenceUnknown, $"'{name}' does not name a part of this document.");
                }
            }
        }
    }
}
