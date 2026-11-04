using AeroTech.ECTM.Application._Shared.Concurrency;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class ConfigurationDocumentLock : IConfigurationDocumentLock
    {
        private readonly IDistributedLock _distributedLock;
        private readonly TimeSpan _expiry;

        public ConfigurationDocumentLock(IDistributedLock distributedLock, IOptions<ResourceLockOptions> options)
        {
            _distributedLock = distributedLock;
            _expiry = TimeSpan.FromSeconds(options.Value.ExpirySeconds);
        }

        public async Task<IAsyncDisposable> AcquireKindAsync(ConfigurationKind kind, CancellationToken cancellationToken = default)
        {
            var resource = $"configuration-kind:{kind}";

            return await _distributedLock.AcquireAsync(resource, _expiry, cancellationToken)
                   ?? throw ExceptionFactory.OperationInProgress(resource);
        }
    }
}
