using System.Text.Json;
using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public static class PhaseReference
    {
        public const string Algorithm = "phase-v1.0.3";
        public const string Title = "Phase algorithm of the pack v3.3 reference (reference/ectm_oracle.py phase())";

        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly Lazy<string> ReferenceLogic = new(() => LogicOf(ReferenceRules()));

        public static PhaseReferenceComparison Compare(PhaseRuleSetContent rules)
        {
            var changes = JsonDiff.Compare(ReferenceLogic.Value, LogicOf(rules));
            var match = changes.Count == 0 ? ReferenceMatch.MATCHES_REFERENCE : ReferenceMatch.MODIFIED_FROM_REFERENCE;

            return new PhaseReferenceComparison(Algorithm, Title, match, changes);
        }

        private static PhaseRuleSetContent ReferenceRules()
            => PhaseRuleSetValidator.Validate(ConfigurationSeeds.ContentOf(ConfigurationKind.PHASE_RULESET), null).Rules
               ?? throw new InvalidOperationException("The PHASE_RULESET seed, which is the phase reference, is invalid.");

        private static string LogicOf(PhaseRuleSetContent rules)
        {
            var logicOnly = rules with
            {
                RuleSetCode = null,
                Title = null,
                Description = null,
                CalibrationStatus = null,
                EngineeringSignOff = null,
                Source = null,
                Decisions = null,
                Events = rules.Events?.Select(definition => definition with { Note = null }).ToList()
            };

            return JsonSerializer.Serialize(logicOnly, Options);
        }
    }
}
