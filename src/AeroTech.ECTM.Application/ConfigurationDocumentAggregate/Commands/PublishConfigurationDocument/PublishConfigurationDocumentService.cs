using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.PublishConfigurationDocument
{
    public sealed class PublishConfigurationDocumentService : IPublishConfigurationDocumentService
    {
        private readonly ConfigurationDocumentLoader _documents;
        private readonly ConfigurationContentValidators _validators;
        private readonly IConfigurationDocumentLock _documentLock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly IActorResolver _actorResolver;

        public PublishConfigurationDocumentService(
            ConfigurationDocumentLoader documents,
            ConfigurationContentValidators validators,
            IConfigurationDocumentLock documentLock,
            IUnitOfWork unitOfWork,
            IClock clock,
            IActorResolver actorResolver)
        {
            _documents = documents;
            _validators = validators;
            _documentLock = documentLock;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _actorResolver = actorResolver;
        }

        public async Task<BackofficeConfigurationDocumentDetailDto> PublishAsync(IPublishConfigurationDocumentCommand command, CancellationToken cancellationToken = default)
        {
            var document = await _documents.LoadAsync(command.Kind, command.DocumentId, cancellationToken);
            var validator = _validators.For(document.Kind);

            await using var kindLock = await _documentLock.AcquireKindAsync(document.Kind, cancellationToken);

            var issues = await validator.ValidateAsync(document.Content, forPublish: true, cancellationToken);

            if (issues.Count > 0)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(document.Id, issues.Count);

            document.Publish(command.ApprovalNote, _actorResolver.Resolve().Subject, _clock.GetDateTime());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BackofficeConfigurationDocumentDetailDto.From(document);
        }
    }
}
