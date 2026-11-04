namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Services
{
    public interface IEngineInstallationLock
    {
        Task<IAsyncDisposable> AcquireEngineAsync(long engineId, CancellationToken cancellationToken = default);

        Task<IAsyncDisposable> AcquirePositionAsync(long aircraftId, int position, CancellationToken cancellationToken = default);
    }
}
