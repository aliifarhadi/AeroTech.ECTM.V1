namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record SnapshotContext(
        IReadOnlyList<ParameterSummary> Conditions,
        IReadOnlyList<string> Toga,
        IReadOnlyList<string> Configurations)
    {
        public static SnapshotContext Of(SnapshotFrame frame, IReadOnlyList<int> indices)
        {
            var conditions = frame.Rules.ConditionParameters!
                .Select(key => frame.SummaryOf(key, indices))
                .ToList();
            var toga = indices
                .Select(index => frame.Text(frame.Rules.Channels!.Toga!, index) ?? string.Empty)
                .Distinct()
                .Order(StringComparer.Ordinal)
                .ToList();
            var configurations = indices
                .Select(frame.ConfigurationAt)
                .Distinct()
                .Order(StringComparer.Ordinal)
                .ToList();

            return new SnapshotContext(conditions, toga, configurations);
        }
    }
}
