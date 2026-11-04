using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.CreateConfigurationDraft
{
    public sealed class CreateConfigurationDraftService : ICreateConfigurationDraftService
    {
        private readonly IConfigurationDocumentRepository _documents;
        private readonly ConfigurationContentValidators _validators;
        private readonly IConfigurationDocumentLock _documentLock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;

        public CreateConfigurationDraftService(
            IConfigurationDocumentRepository documents,
            ConfigurationContentValidators validators,
            IConfigurationDocumentLock documentLock,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator)
        {
            _documents = documents;
            _validators = validators;
            _documentLock = documentLock;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
        }

        public async Task<BackofficeConfigurationDocumentDetailDto> CreateAsync(ICreateConfigurationDraftCommand command, CancellationToken cancellationToken = default)
        {
            var kind = ConfigurationKinds.Parse(command.Kind);
            _validators.For(kind);

            var content = await ContentOfAsync(command, kind, cancellationToken);

            await using var kindLock = await _documentLock.AcquireKindAsync(kind, cancellationToken);

            var nextVersion = await _documents.LastVersionNumberAsync(kind, cancellationToken) + 1;
            var draft = ConfigurationDocument.CreateDraft(_idGenerator.NewId(), kind, nextVersion, command.BasedOnDocumentId, content);

            await _documents.AddAsync(draft, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BackofficeConfigurationDocumentDetailDto.From(draft);
        }

        private async Task<string> ContentOfAsync(ICreateConfigurationDraftCommand command, ConfigurationKind kind, CancellationToken cancellationToken)
        {
            ConfigurationDocument? basedOn = null;

            if (command.BasedOnDocumentId is { } basedOnDocumentId)
            {
                basedOn = await _documents.GetAsync(basedOnDocumentId, cancellationToken);

                if (basedOn is null || basedOn.Kind != kind)
                    throw ExceptionFactory.BasedOnDocumentIsInvalid(basedOnDocumentId, kind);
            }

            if (command.Content is { } content)
                return content.GetRawText();

            if (basedOn is not null)
                return basedOn.Content;

            throw ExceptionFactory.ConfigurationDocumentContentIsRequired();
        }
    }
}
