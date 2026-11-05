using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseReferenceComparison(
        string Algorithm,
        string Title,
        ReferenceMatch Match,
        IReadOnlyList<JsonChange> Changes);
}
