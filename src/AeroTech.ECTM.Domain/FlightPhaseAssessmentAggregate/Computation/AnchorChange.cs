using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record AnchorChange(string Code, int? BeforeIndex, int? AfterIndex, AnchorBasis? BeforeBasis, AnchorBasis? AfterBasis);
}
