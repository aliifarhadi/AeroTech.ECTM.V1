using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record FindingChange(
        string Change,
        string Code,
        string? ParameterKey,
        ValidationSeverity? StoredSeverity,
        long? StoredCount,
        double? StoredValue,
        ValidationSeverity? DryRunSeverity,
        long? DryRunCount,
        double? DryRunValue,
        string Message);
}
