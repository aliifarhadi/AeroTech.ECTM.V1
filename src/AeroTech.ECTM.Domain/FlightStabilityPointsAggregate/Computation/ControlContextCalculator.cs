using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class ControlContextCalculator
    {
        public static ControlContextResult Calculate(SnapshotFrame frame)
        {
            var rules = frame.Rules.ControlContext!;
            var phases = rules.Phases!.Select(phase => SummaryOf(frame, phase)).ToList();

            return new ControlContextResult(rules.VerificationNote!, phases);
        }

        private static ControlPhaseSummary SummaryOf(SnapshotFrame frame, FlightPhaseCode phase)
        {
            var rules = frame.Rules.ControlContext!;
            var indices = frame.Phases.Segments
                .Where(segment => segment.PhaseCode == phase)
                .SelectMany(segment => frame.Range(segment.StartIndex, segment.EndIndexInclusive))
                .ToList();
            var engineChannels = frame.Positions
                .SelectMany(position => rules.EngineChannels!.Select(code => SnapshotFrame.KeyOf(code, position)))
                .Select(key => frame.SummaryOf(key, indices))
                .ToList();
            var modes = rules.ModeChannels!
                .SelectMany(key => ModesOf(frame, key, indices))
                .ToList();

            return new ControlPhaseSummary(
                phase,
                indices.Count,
                indices.Count == 0 ? null : indices[0],
                indices.Count == 0 ? null : indices[^1],
                engineChannels,
                modes);
        }

        private static IEnumerable<ModeObservation> ModesOf(SnapshotFrame frame, string key, IReadOnlyList<int> indices)
            => indices
                .Select(index => (Index: index, Value: frame.Text(key, index)))
                .Where(sample => sample.Value is not null)
                .GroupBy(sample => sample.Value!)
                .Select(group => new ModeObservation(key, group.Key, group.First().Index, group.Count()))
                .OrderBy(mode => mode.FirstIndex);
    }
}
