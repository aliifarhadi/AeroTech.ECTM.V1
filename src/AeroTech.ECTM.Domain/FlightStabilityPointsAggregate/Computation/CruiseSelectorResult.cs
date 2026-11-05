using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record CruiseSelectorResult(
        string Selector,
        int AvailableEngineCount,
        IReadOnlyList<CruiseSnapshot> Engines,
        PairedSnapshot Paired)
    {
        public static CruiseSelectorResult Of(string selector, IReadOnlyList<CruiseSnapshot> engines, PairedSnapshot paired)
            => new(selector, engines.Count(engine => engine.Availability == SnapshotAvailability.AVAILABLE), engines, paired);
    }
}
