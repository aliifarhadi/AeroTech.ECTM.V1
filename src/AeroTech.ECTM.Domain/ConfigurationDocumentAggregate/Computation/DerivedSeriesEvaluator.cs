using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class DerivedSeriesEvaluator
    {
        public static IReadOnlyList<DerivedSeries> Evaluate(ValidatedCatalog catalog, IReadOnlyList<RecordedChannel> channels, int sampleCount)
        {
            var sources = channels
                .Where(channel => channel.IsNumeric)
                .ToDictionary(channel => channel.ParameterKey, channel => new Source(channel.Numbers!, channel.Unit));
            var results = new List<DerivedSeries>();

            foreach (var definition in catalog.DerivedParameters)
            {
                var entry = catalog.Find(definition.Code!)!;

                foreach (var position in PositionsOf(entry))
                {
                    var series = EvaluateOne(definition, catalog.Entries, position, sources, sampleCount);

                    results.Add(series);
                    sources[series.ParameterKey] = new Source(series.Values, definition.UnitOut);
                }
            }

            return results;
        }

        private static DerivedSeries EvaluateOne(
            DerivedParameterDefinition definition,
            IReadOnlyDictionary<string, CatalogEntry> entries,
            int? position,
            IReadOnlyDictionary<string, Source> sources,
            int sampleCount)
        {
            var references = DerivedParameterEvaluator.ReferencesOf(definition)
                .Select(reference => reference.Resolve(position))
                .Distinct(StringComparer.Ordinal)
                .ToList();
            var values = new double?[sampleCount];
            var statusCounts = new Dictionary<string, int>(StringComparer.Ordinal);
            var parameterKey = DataProfileValidator.KeyOf(definition.Code!, position);
            int? firstAvailableSample = null;
            string? firstAvailableFormula = null;

            for (var sample = 0; sample < sampleCount; sample++)
            {
                var inputs = new Dictionary<string, TypedInput>(StringComparer.Ordinal);

                foreach (var reference in references)
                {
                    if (sources.TryGetValue(reference, out var source))
                        inputs[reference] = new TypedInput(source.Values[sample], source.Unit);
                }

                var evaluation = DerivedParameterEvaluator.Evaluate(definition, entries, position, inputs);

                values[sample] = evaluation.Value;
                statusCounts[evaluation.Status] = statusCounts.GetValueOrDefault(evaluation.Status) + 1;

                if (firstAvailableSample is null && evaluation.Status == DerivedParameterEvaluator.Available)
                {
                    firstAvailableSample = sample;
                    firstAvailableFormula = evaluation.Formula;
                }
            }

            return new DerivedSeries(
                parameterKey,
                definition.Code!,
                position,
                definition.Kind!,
                definition.UnitOut,
                definition.Status,
                values,
                statusCounts,
                firstAvailableSample,
                firstAvailableFormula);
        }

        private static IReadOnlyList<int?> PositionsOf(CatalogEntry entry)
        {
            if (entry.Positions.Count == 0)
                return [null];

            return entry.Positions.Select(position => (int?)position).ToList();
        }

        private sealed record Source(IReadOnlyList<double?> Values, string? Unit);
    }
}
