using AeroTech.ECTM.Application._Shared.Concurrency;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.ServiceContracts;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Services
{
    public sealed class EngineInstallationLock : IEngineInstallationLock
    {
        private readonly IDistributedLock _distributedLock;
        private readonly TimeSpan _expiry;

        public EngineInstallationLock(IDistributedLock distributedLock, IOptions<ResourceLockOptions> options)
        {
            _distributedLock = distributedLock;
            _expiry = TimeSpan.FromSeconds(options.Value.ExpirySeconds);
        }

        public Task<IAsyncDisposable> AcquireEngineAsync(long engineId, CancellationToken cancellationToken = default)
            => AcquireAsync($"engine-installation:engine:{engineId}", cancellationToken);

        public Task<IAsyncDisposable> AcquirePositionAsync(long aircraftId, int position, CancellationToken cancellationToken = default)
            => AcquireAsync($"engine-installation:position:{aircraftId}:{position}", cancellationToken);

        private async Task<IAsyncDisposable> AcquireAsync(string resource, CancellationToken cancellationToken)
            => await _distributedLock.AcquireAsync(resource, _expiry, cancellationToken)
               ?? throw ExceptionFactory.OperationInProgress(resource);
    }
}
