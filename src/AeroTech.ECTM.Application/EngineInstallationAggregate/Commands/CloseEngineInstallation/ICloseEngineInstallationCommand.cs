namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation
{
    public interface ICloseEngineInstallationCommand
    {
        long InstallationId { get; }

        DateTimeOffset RemovedUtc { get; }

        string Reason { get; }
    }
}
