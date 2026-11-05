using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "operator")]
    [JsonDerivedType(typeof(FirstUpwardCrossing), FirstUpwardCrossing.Operator)]
    [JsonDerivedType(typeof(SustainedTransition), SustainedTransition.Operator)]
    public abstract record PhaseAnchor(string? Code, AnchorBasis? Basis, IReadOnlyList<AnchorBasis>? FallbackBases)
    {
        private static readonly IReadOnlySet<AnchorBasis> FileEdgeBases = new HashSet<AnchorBasis>
        {
            AnchorBasis.PARTIAL_START,
            AnchorBasis.PARTIAL_END
        };

        public abstract AnchorResult Find(PhaseFrame frame);

        public abstract void Check(PhaseRuleSetCheck check, string path);

        protected AnchorBasis BasisWhenNotFound(PhaseFrame frame)
        {
            foreach (var fallback in FallbackBases!)
            {
                if (FileEdgeHolds(frame, fallback))
                    return fallback;
            }

            return AnchorBasis.NOT_FOUND;
        }

        protected void CheckBases(PhaseRuleSetCheck check, string path)
        {
            check.Required(Basis, $"{path}.basis");

            if (Basis is { } basis && (FileEdgeBases.Contains(basis) || basis == AnchorBasis.NOT_FOUND))
                check.Issue($"{path}.basis", PhaseRuleSetValidator.ValueInvalid, "The basis names the evidence of a found anchor, not a fallback.");

            check.Entries(FallbackBases, $"{path}.fallbackBases", allowEmpty: true);

            for (var item = 0; item < (FallbackBases?.Count ?? 0); item++)
            {
                if (!FileEdgeBases.Contains(FallbackBases![item]))
                    check.Issue($"{path}.fallbackBases[{item}]", PhaseRuleSetValidator.ValueInvalid, "A fallback basis is PARTIAL_START or PARTIAL_END of the quality result.");
            }
        }

        private static bool FileEdgeHolds(PhaseFrame frame, AnchorBasis fallback) => fallback switch
        {
            AnchorBasis.PARTIAL_START => frame.Quality.PartialStart == true,
            AnchorBasis.PARTIAL_END => frame.Quality.PartialEnd == true,
            _ => false
        };
    }
}
