using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed class CruiseWindowGates(SnapshotFrame frame, CruiseSelector selector, int position)
    {
        private readonly IReadOnlyList<double?> _n1 = frame.Numbers(SnapshotFrame.KeyOf(frame.Rules.Channels!.EngineN1!, position));
        private readonly IReadOnlyList<double?> _altitude = frame.Numbers(frame.Rules.Channels!.Altitude!);
        private readonly IReadOnlyList<double?> _mach = frame.Numbers(frame.Rules.Channels!.Mach!);

        public CruiseGate? FirstFailingGate(int windowEnd)
        {
            var windowStart = windowEnd - selector.WindowSec!.Value + 1;

            if (!IsContiguous(windowStart, windowEnd))
                return CruiseGate.NOT_CONTIGUOUS;

            var window = frame.Range(windowStart, windowEnd).ToList();

            if (window.Any(index => !frame.IsCruise(index)))
                return CruiseGate.NOT_CRUISE;

            if (window.Any(index => frame.IsUntrusted(index) || frame.IsCoherenceSuspect(index)))
                return CruiseGate.QUALITY_MASK;

            if (window.Any(index => _n1[index] is null))
                return CruiseGate.N1_MISSING;

            if (N1Deviation(window) > selector.N1TolerancePct)
                return CruiseGate.N1_RANGE;

            if (window.Any(index => _altitude[index] is null || _mach[index] is null))
                return CruiseGate.ALTITUDE_OR_MACH_MISSING;

            if (RangeOf(_altitude, window) > selector.AltitudeRangeFt)
                return CruiseGate.ALTITUDE_RANGE;

            if (RangeOf(_mach, window) > selector.MachRange)
                return CruiseGate.MACH_RANGE;

            if (window.Select(frame.ConfigurationAt).Distinct().Count() != 1)
                return CruiseGate.CONFIGURATION_CHANGED;

            return null;
        }

        public IReadOnlyList<GateValue> Measure(int start, int end)
        {
            var window = frame.Range(start, end).ToList();
            var configurations = window.Select(frame.ConfigurationAt).Distinct().Count();

            return
            [
                Gate(CruiseGate.N1_RANGE, N1Deviation(window), selector.N1TolerancePct!.Value),
                Gate(CruiseGate.ALTITUDE_RANGE, RangeOf(_altitude, window), selector.AltitudeRangeFt!.Value),
                Gate(CruiseGate.MACH_RANGE, RangeOf(_mach, window), selector.MachRange!.Value),
                Gate(CruiseGate.CONFIGURATION_CHANGED, configurations, 1)
            ];
        }

        private bool IsContiguous(int windowStart, int windowEnd)
        {
            if (windowStart < 0 || !frame.SameBlock(windowStart, windowEnd))
                return false;

            return frame.Elapsed[windowEnd] - frame.Elapsed[windowStart] == selector.WindowSec - 1;
        }

        private double N1Deviation(IReadOnlyList<int> window)
        {
            var mean = CompensatedSum.Of(window.Select(index => _n1[index] ?? 0)) / window.Count;

            return window.Max(index => Math.Abs((_n1[index] ?? mean) - mean));
        }

        private static double RangeOf(IReadOnlyList<double?> values, IReadOnlyList<int> window)
        {
            var present = window.Select(index => values[index]).OfType<double>().ToList();

            return present.Count == 0 ? 0 : present.Max() - present.Min();
        }

        private static GateValue Gate(CruiseGate gate, double actual, double limit) => new(gate, actual, limit, actual <= limit);
    }
}
