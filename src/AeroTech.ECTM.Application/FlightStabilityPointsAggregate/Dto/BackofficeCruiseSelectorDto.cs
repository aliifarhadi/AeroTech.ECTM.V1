using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeCruiseSelectorDto(
        string Selector,
        int AvailableEngineCount,
        IReadOnlyList<BackofficeCruiseSnapshotDto> Engines,
        BackofficePairedSnapshotDto Paired)
    {
        public static BackofficeCruiseSelectorDto From(CruiseSelectorResult selector)
            => new(selector.Selector, selector.AvailableEngineCount, selector.Engines.Select(BackofficeCruiseSnapshotDto.From).ToList(), BackofficePairedSnapshotDto.From(selector.Paired));
    }
}
