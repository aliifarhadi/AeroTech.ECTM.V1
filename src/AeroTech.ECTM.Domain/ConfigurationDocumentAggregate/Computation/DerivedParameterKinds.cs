namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class DerivedParameterKinds
    {
        public const string Difference = "DIFFERENCE";
        public const string Ratio = "RATIO";
        public const string LinearCombination = "LINEAR_COMBINATION";
        public const string PowerRatio = "POWER_RATIO";
        public const string UnitConversion = "UNIT_CONVERSION";
        public const string Lookup1D = "LOOKUP_1D";
        public const string Lookup2D = "LOOKUP_2D";
        public const string CentralDifferenceRate = "CENTRAL_DIFFERENCE_RATE";
        public const string MedianSmooth = "MEDIAN_SMOOTH";
        public const string WindowStatistic = "WINDOW_STATISTIC";

        public const string AvailableNow = "P02";

        public static readonly IReadOnlyList<DerivedParameterKind> All =
        [
            new(Difference, ["a", "b"], AvailableNow),
            new(Ratio, ["numerator", "denominator", "guardZero"], AvailableNow),
            new(LinearCombination, ["terms", "offset"], AvailableNow),
            new(PowerRatio, ["ref", "referenceConstant", "exponentConstantRef"], AvailableNow),
            new(UnitConversion, ["ref", "fromUnit", "toUnit"], "not scheduled: no unit conversion registry yet"),
            new(Lookup1D, ["tableRef", "axisRef", "interpolation", "extrapolation"], "P08"),
            new(Lookup2D, ["tableRef", "axisRefs", "interpolation", "extrapolation"], "P08"),
            new(CentralDifferenceRate, ["ref", "halfWindowSec", "toleranceSec", "unitOut"], "P05 (PHASE_RULESET derived channels)"),
            new(MedianSmooth, ["ref", "points", "minPoints", "nullInsideMask"], "P05 (PHASE_RULESET derived channels)"),
            new(WindowStatistic, ["ref", "statistic"], "P06")
        ];

        public static DerivedParameterKind? Find(string? code)
            => All.FirstOrDefault(kind => kind.Code == code);
    }
}
