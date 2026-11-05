using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public static class Comparison
    {
        public static bool Holds(double observed, CompareOperator op, double value) => op switch
        {
            CompareOperator.LT => observed < value,
            CompareOperator.LE => observed <= value,
            CompareOperator.GT => observed > value,
            CompareOperator.GE => observed >= value,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, "Unknown comparison.")
        };
    }
}
