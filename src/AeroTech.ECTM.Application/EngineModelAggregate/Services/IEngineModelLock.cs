namespace AeroTech.ECTM.Application.EngineModelAggregate.Services
{
    public interface IEngineModelLock
    {
        Task<IAsyncDisposable> AcquireCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
