using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class ConfigurationSeeder : IConfigurationSeeder
    {
        private readonly IConfigurationDocumentRepository _documents;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;

        public ConfigurationSeeder(IConfigurationDocumentRepository documents, IUnitOfWork unitOfWork, IIdGenerator idGenerator)
        {
            _documents = documents;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
        }

        public async Task SeedMissingAsync(CancellationToken cancellationToken = default)
        {
            foreach (var kind in ConfigurationSeeds.ResourceByKind.Keys)
            {
                var lastVersion = await _documents.LastVersionNumberAsync(kind, cancellationToken);

                if (lastVersion > 0)
                    continue;

                var seedDraft = ConfigurationDocument.CreateDraft(_idGenerator.NewId(), kind, 1, null, ConfigurationSeeds.ContentOf(kind));

                await _documents.AddAsync(seedDraft, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
