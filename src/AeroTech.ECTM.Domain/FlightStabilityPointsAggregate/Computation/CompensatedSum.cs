namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class CompensatedSum
    {
        public static double Of(IEnumerable<double> values)
        {
            var sum = 0.0;
            var compensation = 0.0;

            foreach (var value in values)
            {
                var total = sum + value;

                if (Math.Abs(sum) >= Math.Abs(value))
                    compensation += sum - total + value;
                else
                    compensation += value - total + sum;

                sum = total;
            }

            return compensation != 0 && double.IsFinite(compensation) ? sum + compensation : sum;
        }
    }
}
