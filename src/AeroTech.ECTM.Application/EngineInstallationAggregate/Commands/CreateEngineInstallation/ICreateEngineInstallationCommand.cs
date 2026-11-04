namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation
{
    public interface ICreateEngineInstallationCommand
    {
        long EngineId { get; }

        long AircraftId { get; }

        int Position { get; }

        DateTimeOffset InstalledUtc { get; }

        string Reason { get; }

        string? Evidence { get; }
    }
}
