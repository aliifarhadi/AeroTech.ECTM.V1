using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "operator")]
    [JsonDerivedType(typeof(CentralDifferenceRate), CentralDifferenceRate.Operator)]
    [JsonDerivedType(typeof(MedianSmooth), MedianSmooth.Operator)]
    public abstract record DerivedChannel(string? Code)
    {
        public abstract IReadOnlyList<double?> Compute(PhaseFrame frame);

        public abstract DerivedValueTrace Trace(PhaseFrame frame, int index);

        public abstract void Check(RuleDocumentCheck check, string path);
    }
}
