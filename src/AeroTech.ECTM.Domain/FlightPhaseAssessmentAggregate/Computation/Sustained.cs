using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record Sustained(string? Code, double? MinSec, PhasePredicate? Predicate) : PhaseCondition(Code)
    {
        public const string Operator = "SUSTAINED";

        public override bool[] Members(PhaseFrame frame)
        {
            var candidates = frame.SamplesWhere(index => Predicate!.Holds(frame, index));
            var members = new bool[frame.Count];

            foreach (var (start, end) in frame.RunsOf(candidates))
            {
                if (frame.RunSeconds(start, end) >= MinSec)
                    PhaseFrame.Mark(members, start, end);
            }

            return members;
        }

        public override ConditionTrace Trace(PhaseFrame frame, int index)
        {
            var predicate = Predicate!.Trace(frame, index);
            var contains = frame.InCondition(Code!, index);

            if (!predicate.Holds)
                return new ConditionTrace(Code!, Operator, contains, predicate, RequiredSec: MinSec);

            var (start, end) = frame.RunAround(index, sample => Predicate.Holds(frame, sample));

            return new ConditionTrace(Code!, Operator, contains, predicate, start, end, frame.RunSeconds(start, end), MinSec);
        }

        public override void Check(PhaseRuleSetCheck check, string path)
        {
            check.Positive(MinSec, $"{path}.minSec");
            PhasePredicate.CheckPresent(Predicate, check, $"{path}.predicate");
        }
    }
}
