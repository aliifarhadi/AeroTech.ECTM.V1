namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record TakeoffRules(
        string? LiftoffAnchor,
        double? SearchBeforeLiftoffSec,
        double? SearchAfterLiftoffSec,
        double? PartialStartN1Pct,
        double? WindowBeforePeakSec,
        double? WindowAfterPeakSec,
        int? MinValidN1Samples);
}
