namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ClockRules(
        double? StepToleranceSec,
        bool? AcceptOneDigitHour);
}
