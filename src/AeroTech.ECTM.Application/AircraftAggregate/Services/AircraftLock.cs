using AeroTech.ECTM.Application._Shared.Concurrency;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.ServiceContracts;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Application.AircraftAggregate.Services
{
    public sealed class AircraftLock : IAircraftLock
    {
        private readonly IDistributedLock _distributedLock;
        private readonly TimeSpan _expiry;

        public AircraftLock(IDistributedLock distributedLock, IOptions<ResourceLockOptions> options)
        {
            _distributedLock = distributedLock;
            _expiry = TimeSpan.FromSeconds(options.Value.ExpirySeconds);
        }

        public async Task<IAsyncDisposable> AcquireRegistrationAsync(string registration, CancellationToken cancellationToken = default)
        {
            var resource = $"aircraft-registration:{registration}";

            return await _distributedLock.AcquireAsync(resource, _expiry, cancellationToken)
                   ?? throw ExceptionFactory.OperationInProgress(resource);
        }
    }
}
