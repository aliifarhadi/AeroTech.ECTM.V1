namespace AeroTech.ECTM.Domain.EngineInstallationAggregate.Arguments
{
    public sealed record InstallationFilterArgs(
        long? EngineId,
        long? AircraftId,
        int? Position,
        bool OpenOnly);
}
