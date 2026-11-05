using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record PeriodPerformance(
        int Contributors,
        IReadOnlyList<ValueDistribution> Parameters,
        IReadOnlyList<ValueDistribution> Conditions,
        IReadOnlyDictionary<string, int> Configurations)
    {
        public static PeriodPerformance Of(IReadOnlyList<SnapshotObservation> observations)
        {
            var parameters = DistributionsOf(observations.SelectMany(observation => observation.Values));
            var conditions = DistributionsOf(observations.SelectMany(observation => observation.Conditions));
            var configurations = observations
                .SelectMany(observation => observation.Configurations)
                .GroupBy(configuration => configuration)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.Count());

            return new PeriodPerformance(observations.Count, parameters, conditions, configurations);
        }

        private static List<ValueDistribution> DistributionsOf(IEnumerable<ParameterSummary> summaries)
            => summaries
                .Where(summary => summary.Mean is not null)
                .GroupBy(summary => CodeOf(summary.ParameterKey))
                .Select(group => ValueDistribution.Of(group.Key, group.Select(summary => summary.Mean!.Value)))
                .ToList();

        private static string CodeOf(string parameterKey) => ParameterReference.Parse(parameterKey)?.Code ?? parameterKey;
    }
}
