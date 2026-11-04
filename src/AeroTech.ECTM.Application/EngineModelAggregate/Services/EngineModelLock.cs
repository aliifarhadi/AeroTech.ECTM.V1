using AeroTech.ECTM.Application._Shared.Concurrency;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.ServiceContracts;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Services
{
    public sealed class EngineModelLock : IEngineModelLock
    {
        private readonly IDistributedLock _distributedLock;
        private readonly TimeSpan _expiry;

        public EngineModelLock(IDistributedLock distributedLock, IOptions<ResourceLockOptions> options)
        {
            _distributedLock = distributedLock;
            _expiry = TimeSpan.FromSeconds(options.Value.ExpirySeconds);
        }

        public async Task<IAsyncDisposable> AcquireCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var resource = $"engine-model-code:{code.ToUpperInvariant()}";

            return await _distributedLock.AcquireAsync(resource, _expiry, cancellationToken)
                   ?? throw ExceptionFactory.OperationInProgress(resource);
        }
    }
}
