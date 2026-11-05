namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class M4Reducer
    {
        public const int PointsPerBucket = 4;

        public static IReadOnlyList<T> Reduce<T>(IReadOnlyList<T> points, int maxPoints, Func<T, double> value)
        {
            if (points.Count <= maxPoints)
                return points;

            var buckets = Math.Max(1, maxPoints / PointsPerBucket);
            var kept = new SortedSet<int>();

            for (var bucket = 0; bucket < buckets; bucket++)
            {
                var first = (int)((long)bucket * points.Count / buckets);
                var last = (int)(((long)bucket + 1) * points.Count / buckets) - 1;
                var positions = Enumerable.Range(first, last - first + 1).ToList();

                kept.Add(first);
                kept.Add(last);
                kept.Add(positions.MinBy(position => value(points[position])));
                kept.Add(positions.MaxBy(position => value(points[position])));
            }

            return kept.Select(position => points[position]).ToList();
        }
    }
}
