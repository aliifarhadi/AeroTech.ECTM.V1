using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public static class PhaseSegmenter
    {
        public const string EdgeWindow = "EDGE_WINDOW";
        public const string ChannelCoherenceSuspect = "CHANNEL_COHERENCE_SUSPECT";

        public static IReadOnlyList<PhaseSegment> Segments(SegmentRules rules, PhaseFrame frame, PhaseLabels labels)
            => labels.Runs(frame)
                .Select(run => SegmentOf(rules, frame, labels, run))
                .ToList();

        private static PhaseSegment SegmentOf(SegmentRules rules, PhaseFrame frame, PhaseLabels labels, LabelRun run)
        {
            var atEdge = run.Start == 0 || run.End == frame.Count - 1;
            var coherenceSuspect = rules.CoherenceSuspectMasks!.Any(mask => frame.OverlapsMask(mask, run.Start, run.End));
            var flags = new List<string>();

            if (atEdge)
                flags.Add(EdgeWindow);

            if (coherenceSuspect)
                flags.Add(ChannelCoherenceSuspect);

            return new PhaseSegment(
                run.Label,
                run.Start,
                run.End,
                frame.Elapsed[run.Start],
                frame.Elapsed[run.End],
                frame.RunSeconds(run.Start, run.End),
                ConfidenceOf(rules, run, atEdge, coherenceSuspect),
                rules.EvidenceCodes![run.Label],
                flags,
                UnknownReasonsOf(labels, run));
        }

        private static PhaseConfidence ConfidenceOf(SegmentRules rules, LabelRun run, bool atEdge, bool coherenceSuspect)
        {
            if (run.Label == FlightPhaseCode.UNKNOWN || coherenceSuspect)
                return PhaseConfidence.LOW;

            if (atEdge && !rules.EdgeExemptLabels!.Contains(run.Label))
                return PhaseConfidence.LOW;

            if (rules.MediumConfidenceLabels!.Contains(run.Label))
                return PhaseConfidence.MEDIUM;

            return PhaseConfidence.HIGH;
        }

        private static IReadOnlyList<PhaseUnknownReason> UnknownReasonsOf(PhaseLabels labels, LabelRun run)
        {
            if (run.Label != FlightPhaseCode.UNKNOWN)
                return [];

            var reasons = new HashSet<PhaseUnknownReason>();

            for (var index = run.Start; index <= run.End; index++)
            {
                if (labels.UnknownReasons[index] is { } reason)
                    reasons.Add(reason);
            }

            return reasons
                .OrderBy(reason => reason.ToString(), StringComparer.Ordinal)
                .ToList();
        }
    }
}
