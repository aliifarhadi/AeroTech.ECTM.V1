namespace AeroTech.ECTM.Application.AircraftAggregate.Services
{
    public interface IAircraftLock
    {
        Task<IAsyncDisposable> AcquireRegistrationAsync(string registration, CancellationToken cancellationToken = default);
    }
}
