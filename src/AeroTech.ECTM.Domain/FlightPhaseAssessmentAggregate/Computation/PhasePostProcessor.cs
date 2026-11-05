using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "operator")]
    [JsonDerivedType(typeof(AdjacentGapUnknown), AdjacentGapUnknown.Operator)]
    [JsonDerivedType(typeof(DropShortIsland), DropShortIsland.Operator)]
    [JsonDerivedType(typeof(UncorroboratedClimbInDescentUnknown), UncorroboratedClimbInDescentUnknown.Operator)]
    public abstract record PhasePostProcessor
    {
        [JsonIgnore]
        public abstract string Name { get; }

        public abstract void Apply(PhaseFrame frame, PhaseLabels labels);

        public abstract void Check(RuleDocumentCheck check, string path);
    }
}
