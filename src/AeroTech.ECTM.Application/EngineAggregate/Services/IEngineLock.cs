namespace AeroTech.ECTM.Application.EngineAggregate.Services
{
    public interface IEngineLock
    {
        Task<IAsyncDisposable> AcquireSerialAsync(string manufacturer, string serialNumber, CancellationToken cancellationToken = default);
    }
}
