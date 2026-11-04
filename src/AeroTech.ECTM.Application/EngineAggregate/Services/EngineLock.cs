using AeroTech.ECTM.Application._Shared.Concurrency;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.ServiceContracts;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Application.EngineAggregate.Services
{
    public sealed class EngineLock : IEngineLock
    {
        private readonly IDistributedLock _distributedLock;
        private readonly TimeSpan _expiry;

        public EngineLock(IDistributedLock distributedLock, IOptions<ResourceLockOptions> options)
        {
            _distributedLock = distributedLock;
            _expiry = TimeSpan.FromSeconds(options.Value.ExpirySeconds);
        }

        public async Task<IAsyncDisposable> AcquireSerialAsync(string manufacturer, string serialNumber, CancellationToken cancellationToken = default)
        {
            var resource = $"engine-serial:{manufacturer.ToUpperInvariant()}:{serialNumber.ToUpperInvariant()}";

            return await _distributedLock.AcquireAsync(resource, _expiry, cancellationToken)
                   ?? throw ExceptionFactory.OperationInProgress(resource);
        }
    }
}
