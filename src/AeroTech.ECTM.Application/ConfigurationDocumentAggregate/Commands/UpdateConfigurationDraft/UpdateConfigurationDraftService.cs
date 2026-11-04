using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.Framework.Core.Domain.Repository;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.UpdateConfigurationDraft
{
    public sealed class UpdateConfigurationDraftService : IUpdateConfigurationDraftService
    {
        private readonly ConfigurationDocumentLoader _documents;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateConfigurationDraftService(ConfigurationDocumentLoader documents, IUnitOfWork unitOfWork)
        {
            _documents = documents;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeConfigurationDocumentDetailDto> UpdateAsync(IUpdateConfigurationDraftCommand command, CancellationToken cancellationToken = default)
        {
            var document = await _documents.LoadAsync(command.Kind, command.DocumentId, cancellationToken);

            document.UpdateDraftContent(command.Content.GetRawText());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BackofficeConfigurationDocumentDetailDto.From(document);
        }
    }
}
