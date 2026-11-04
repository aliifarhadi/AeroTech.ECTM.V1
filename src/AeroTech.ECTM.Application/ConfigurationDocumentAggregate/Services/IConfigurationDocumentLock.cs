using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public interface IConfigurationDocumentLock
    {
        Task<IAsyncDisposable> AcquireKindAsync(ConfigurationKind kind, CancellationToken cancellationToken = default);
    }
}
