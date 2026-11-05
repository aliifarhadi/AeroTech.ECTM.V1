namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class FindingComparison
    {
        public const string Added = "ADDED";
        public const string Removed = "REMOVED";
        public const string Changed = "CHANGED";
        public const string Unchanged = "UNCHANGED";

        public static IReadOnlyList<FindingChange> Compare(IReadOnlyList<ValidationFinding> stored, IReadOnlyList<ValidationFinding> dryRun)
        {
            var storedByKey = stored.ToDictionary(KeyOf);
            var dryRunByKey = dryRun.ToDictionary(KeyOf);
            var keys = storedByKey.Keys.Union(dryRunByKey.Keys).ToList();
            var changes = new List<FindingChange>();

            foreach (var key in keys)
            {
                var before = storedByKey.GetValueOrDefault(key);
                var after = dryRunByKey.GetValueOrDefault(key);
                var shown = after ?? before!;

                changes.Add(new FindingChange(
                    ChangeOf(before, after),
                    shown.Code,
                    shown.ParameterKey,
                    before?.Severity,
                    before?.Count,
                    before?.Value,
                    after?.Severity,
                    after?.Count,
                    after?.Value,
                    shown.Message));
            }

            return changes
                .OrderBy(change => change.Change == Unchanged)
                .ThenBy(change => change.Code, StringComparer.Ordinal)
                .ThenBy(change => change.ParameterKey, StringComparer.Ordinal)
                .ToList();
        }

        private static string ChangeOf(ValidationFinding? before, ValidationFinding? after)
        {
            if (before is null)
                return Added;

            if (after is null)
                return Removed;

            var same = before.Severity == after.Severity && before.Count == after.Count && before.Value == after.Value;

            return same ? Unchanged : Changed;
        }

        private static string KeyOf(ValidationFinding finding) => $"{finding.Code}|{finding.ParameterKey}";
    }
}
