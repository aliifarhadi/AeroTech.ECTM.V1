using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record CruiseRules(FlightPhaseCode? Phase, IReadOnlyList<CruiseSelector>? Selectors);
}
