namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record CruiseSelector(
        string? Code,
        double? N1TolerancePct,
        double? AltitudeRangeFt,
        double? MachRange,
        int? WindowSec,
        double? MinBlockSec,
        int? ReportSec);
}
