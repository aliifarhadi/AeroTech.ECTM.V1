namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record DerivedSeries(
        string ParameterKey,
        string Code,
        int? Position,
        string Kind,
        string? UnitOut,
        string? DefinitionStatus,
        IReadOnlyList<double?> Values,
        IReadOnlyDictionary<string, int> StatusCounts,
        int? FirstAvailableSample,
        string? FirstAvailableFormula)
    {
        public int AvailableCount => StatusCounts.GetValueOrDefault(DerivedParameterEvaluator.Available);

        public double? Minimum => Values.Where(value => value is not null).DefaultIfEmpty().Min();

        public double? Maximum => Values.Where(value => value is not null).DefaultIfEmpty().Max();

        public bool SameValuesAs(DerivedSeries other)
        {
            if (Values.Count != other.Values.Count)
                return false;

            for (var index = 0; index < Values.Count; index++)
            {
                if (BitsOf(Values[index]) != BitsOf(other.Values[index]))
                    return false;
            }

            return true;
        }

        private static long? BitsOf(double? value) => value is { } number ? BitConverter.DoubleToInt64Bits(number) : null;
    }
}
