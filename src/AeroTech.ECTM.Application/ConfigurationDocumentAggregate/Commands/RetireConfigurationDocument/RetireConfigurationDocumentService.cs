using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.RetireConfigurationDocument
{
    public sealed class RetireConfigurationDocumentService : IRetireConfigurationDocumentService
    {
        private readonly ConfigurationDocumentLoader _documents;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly IActorResolver _actorResolver;

        public RetireConfigurationDocumentService(ConfigurationDocumentLoader documents, IUnitOfWork unitOfWork, IClock clock, IActorResolver actorResolver)
        {
            _documents = documents;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _actorResolver = actorResolver;
        }

        public async Task<BackofficeConfigurationDocumentDetailDto> RetireAsync(IRetireConfigurationDocumentCommand command, CancellationToken cancellationToken = default)
        {
            var document = await _documents.LoadAsync(command.Kind, command.DocumentId, cancellationToken);

            document.Retire(command.Note, _actorResolver.Resolve().Subject, _clock.GetDateTime());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BackofficeConfigurationDocumentDetailDto.From(document);
        }
    }
}
