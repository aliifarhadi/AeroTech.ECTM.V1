using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "operator")]
    [JsonDerivedType(typeof(SamplesWhere), SamplesWhere.Operator)]
    [JsonDerivedType(typeof(Sustained), Sustained.Operator)]
    [JsonDerivedType(typeof(AnchorWindow), AnchorWindow.Operator)]
    [JsonDerivedType(typeof(BackFromAnchorWhile), BackFromAnchorWhile.Operator)]
    [JsonDerivedType(typeof(FromAnchorWhile), FromAnchorWhile.Operator)]
    public abstract record PhaseCondition(string? Code)
    {
        public abstract bool[] Members(PhaseFrame frame);

        public abstract ConditionTrace Trace(PhaseFrame frame, int index);

        public abstract void Check(PhaseRuleSetCheck check, string path);
    }
}
