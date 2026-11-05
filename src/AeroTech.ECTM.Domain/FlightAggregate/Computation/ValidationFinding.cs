using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record ValidationFinding(
        ValidationSeverity Severity,
        string Code,
        string Message,
        string? ParameterKey,
        long? Count,
        double? Value);
}
