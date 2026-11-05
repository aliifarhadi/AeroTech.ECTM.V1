namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class SeriesSampler
    {
        public const string Raw = "RAW";
        public const string M4 = "M4";

        public static SampledSeries Sample(IReadOnlyList<double?> elapsed, IReadOnlyList<double?> values, int from, int to, string sampling, int budget)
        {
            var axis = TimeAxis.Of(elapsed);
            var gaps = axis.Gaps
                .Where(gap => gap.AfterSampleIndex >= from && gap.AfterSampleIndex < to)
                .ToList();
            var gapAfter = gaps.ToDictionary(gap => gap.AfterSampleIndex);
            var pieces = PiecesOf(elapsed, values, from, to, gapAfter, axis.NominalStep);
            var sampleCountInRange = to - from + 1;

            if (sampling == Raw)
                return new SampledSeries(sampleCountInRange, pieces.SelectMany(piece => piece.Points).ToList(), gaps);

            var valuedCount = pieces.Where(piece => piece.IsRun).Sum(piece => piece.Points.Count);
            var points = new List<SeriesPoint>();

            foreach (var piece in pieces)
            {
                if (!piece.IsRun)
                {
                    var previousIsBreak = points.Count > 0 && points[^1].Value is null;

                    if (!previousIsBreak)
                        points.AddRange(piece.Points);

                    continue;
                }

                var share = (int)((long)budget * piece.Points.Count / Math.Max(1, valuedCount));
                var runBudget = Math.Max(M4Reducer.PointsPerBucket, share);

                points.AddRange(M4Reducer.Reduce(piece.Points, runBudget, point => point.Value!.Value));
            }

            return new SampledSeries(sampleCountInRange, points, gaps);
        }

        private static List<Piece> PiecesOf(IReadOnlyList<double?> elapsed, IReadOnlyList<double?> values, int from, int to, IReadOnlyDictionary<int, TimeGap> gapAfter, double? nominalStep)
        {
            var pieces = new List<Piece>();
            var run = new List<SeriesPoint>();

            for (var index = from; index <= to; index++)
            {
                if (elapsed[index] is not { } time)
                {
                    CloseRun(run, pieces);
                    continue;
                }

                if (values[index] is { } value)
                    run.Add(new SeriesPoint(index, time, value));
                else
                {
                    CloseRun(run, pieces);
                    pieces.Add(Piece.Break(new SeriesPoint(index, time, null)));
                }

                if (gapAfter.TryGetValue(index, out var gap))
                {
                    CloseRun(run, pieces);
                    pieces.Add(Piece.Break(new SeriesPoint(null, gap.ElapsedFrom + nominalStep!.Value, null)));
                }
            }

            CloseRun(run, pieces);

            return pieces;
        }

        private static void CloseRun(List<SeriesPoint> run, List<Piece> pieces)
        {
            if (run.Count == 0)
                return;

            pieces.Add(new Piece(true, run.ToList()));
            run.Clear();
        }

        private sealed record Piece(bool IsRun, IReadOnlyList<SeriesPoint> Points)
        {
            public static Piece Break(SeriesPoint point) => new(false, [point]);
        }
    }
}
