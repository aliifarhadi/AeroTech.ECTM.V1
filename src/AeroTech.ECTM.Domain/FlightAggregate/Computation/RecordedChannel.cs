namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record RecordedChannel(
        int Ordinal,
        string ParameterKey,
        string ParameterCode,
        int? Position,
        string Unit,
        string ValueKind,
        IReadOnlyList<double?>? Numbers,
        IReadOnlyList<string?>? Texts,
        int MissingCount,
        int UnparseableCount,
        int OutOfRangeCount,
        IReadOnlyDictionary<string, int> LiteralsOutsideDomain)
    {
        public bool IsNumeric => Numbers is not null;

        public int SampleCount => Numbers?.Count ?? Texts!.Count;

        public int ValuedCount => SampleCount - MissingCount - UnparseableCount;
    }
}
