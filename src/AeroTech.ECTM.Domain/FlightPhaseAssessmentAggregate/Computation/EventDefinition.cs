using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record EventDefinition(
        string? Code,
        string? Parameter,
        string? ActiveValue,
        bool? Enabled,
        IReadOnlyList<string>? ValuesAtStartAndEnd,
        IReadOnlyList<string>? PeakDuring,
        string? Note)
    {
        public void Check(PhaseRuleSetCheck check, string path)
        {
            check.Text(Code, $"{path}.code");
            check.Required(Enabled, $"{path}.enabled");

            if (check.ParameterOf(Parameter, $"{path}.parameter") is { IsNumeric: true })
                check.Issue($"{path}.parameter", PhaseRuleSetValidator.ChannelKindInvalid, $"{Parameter} must be a text or enumerated parameter.");

            if (Enabled == true || ActiveValue is not null)
                check.Literal(Parameter, ActiveValue, $"{path}.activeValue");

            if (Enabled == false)
                check.Text(Note, $"{path}.note");

            NumericParameters(ValuesAtStartAndEnd, check, $"{path}.valuesAtStartAndEnd");
            NumericParameters(PeakDuring, check, $"{path}.peakDuring");
        }

        private static void NumericParameters(IReadOnlyList<string>? codes, PhaseRuleSetCheck check, string path)
        {
            check.Entries(codes, path, allowEmpty: true);

            for (var item = 0; item < (codes?.Count ?? 0); item++)
            {
                if (check.ParameterOf(codes![item], $"{path}[{item}]") is { IsNumeric: false })
                    check.Issue($"{path}[{item}]", PhaseRuleSetValidator.ChannelKindInvalid, $"{codes[item]} must be numeric.");
            }
        }
    }
}
