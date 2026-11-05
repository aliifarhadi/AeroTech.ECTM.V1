using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record LabelRule(
        string? Id,
        FlightPhaseCode? Label,
        PhaseUnknownReason? UnknownReason,
        PhasePredicate? When,
        AnchorLabel? BeforeAnchor,
        AnchorLabel? AfterAnchor)
    {
        public bool Matches(PhaseFrame frame, int index) => When!.Holds(frame, index);

        public FlightPhaseCode LabelAt(PhaseFrame frame, int index)
        {
            if (BeforeAnchor is { } before && frame.AnchorIndex(before.Anchor!) is { } beforeIndex && index < beforeIndex)
                return before.Label!.Value;

            if (AfterAnchor is { } after && frame.AnchorIndex(after.Anchor!) is { } afterIndex && index > afterIndex)
                return after.Label!.Value;

            return Label!.Value;
        }

        public void Check(PhaseRuleSetCheck check, string path)
        {
            check.Text(Id, $"{path}.id");
            check.Label(Label, UnknownReason, path);
            PhasePredicate.CheckPresent(When, check, $"{path}.when");
            CheckAnchorLabel(BeforeAnchor, check, $"{path}.beforeAnchor");
            CheckAnchorLabel(AfterAnchor, check, $"{path}.afterAnchor");
        }

        private void CheckAnchorLabel(AnchorLabel? anchorLabel, PhaseRuleSetCheck check, string path)
        {
            if (anchorLabel is null)
                return;

            check.Anchor(anchorLabel.Anchor, $"{path}.anchor");
            check.Required(anchorLabel.Label, $"{path}.label");

            if (anchorLabel.Label is not { } label)
                return;

            if (label == FlightPhaseCode.UNKNOWN || UnknownReason is not null)
                check.Issue($"{path}.label", PhaseRuleSetValidator.ValueInvalid, "An anchor sub-label is a known phase of a rule without an unknown reason.");
            else
                check.ProducesLabel(label);
        }
    }
}
