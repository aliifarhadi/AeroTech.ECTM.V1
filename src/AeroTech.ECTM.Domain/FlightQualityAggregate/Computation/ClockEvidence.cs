using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed record ClockEvidence(
        int DisagreeRows,
        int InvalidOrMissingRows,
        int PrimaryNotInStepRows,
        int SecondaryNotInStepRows,
        IReadOnlyDictionary<string, int> OffsetHistogram,
        TimeTrust TimeTrust);
}
