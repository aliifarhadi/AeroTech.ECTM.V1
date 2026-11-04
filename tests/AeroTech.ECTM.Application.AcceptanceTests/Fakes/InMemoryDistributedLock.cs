using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fakes;

public sealed class InMemoryDistributedLock : IDistributedLock
{
    private readonly HashSet<string> _held = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public Task<IAsyncDisposable?> AcquireAsync(string resource, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (!_held.Add(resource))
                return Task.FromResult<IAsyncDisposable?>(null);
        }

        return Task.FromResult<IAsyncDisposable?>(new Handle(() => Release(resource)));
    }

    private void Release(string resource)
    {
        lock (_gate)
            _held.Remove(resource);
    }

    private sealed class Handle(Action release) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            release();
            return ValueTask.CompletedTask;
        }
    }
}
