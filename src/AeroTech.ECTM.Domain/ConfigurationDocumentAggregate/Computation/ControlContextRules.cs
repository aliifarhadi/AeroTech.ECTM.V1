using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ControlContextRules(
        IReadOnlyList<FlightPhaseCode>? Phases,
        IReadOnlyList<string>? EngineChannels,
        IReadOnlyList<string>? ModeChannels,
        string? VerificationNote);
}
