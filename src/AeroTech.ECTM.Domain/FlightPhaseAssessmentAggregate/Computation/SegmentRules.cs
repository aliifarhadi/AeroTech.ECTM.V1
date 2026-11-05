using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record SegmentRules(
        IReadOnlyList<string>? CoherenceSuspectMasks,
        IReadOnlyList<FlightPhaseCode>? EdgeExemptLabels,
        IReadOnlyList<FlightPhaseCode>? MediumConfidenceLabels,
        IReadOnlyDictionary<FlightPhaseCode, IReadOnlyList<string>>? EvidenceCodes)
    {
        public void Check(RuleDocumentCheck check, string path)
        {
            check.Entries(CoherenceSuspectMasks, $"{path}.coherenceSuspectMasks", allowEmpty: true);

            for (var item = 0; item < (CoherenceSuspectMasks?.Count ?? 0); item++)
                check.Mask(CoherenceSuspectMasks![item], $"{path}.coherenceSuspectMasks[{item}]");

            check.Entries(EdgeExemptLabels, $"{path}.edgeExemptLabels", allowEmpty: true);
            check.Entries(MediumConfidenceLabels, $"{path}.mediumConfidenceLabels", allowEmpty: true);

            if (EvidenceCodes is null)
            {
                check.Required(null, $"{path}.evidenceCodes");
                return;
            }

            foreach (var label in check.Labels.Order())
            {
                var codes = EvidenceCodes.GetValueOrDefault(label);

                if (codes is null || codes.Count == 0 || codes.Any(string.IsNullOrWhiteSpace))
                    check.Issue($"{path}.evidenceCodes.{label}", RuleDocumentCheck.FieldRequired, $"The label {label} can occur and needs its evidence codes.");
            }
        }
    }
}
