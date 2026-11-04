using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.PreviewDataProfile.Backoffice
{
    public sealed class BackofficePreviewDataProfileQueryHandler : IRequestHandler<BackofficePreviewDataProfileQuery, BackofficeProfilePreviewDto>
    {
        private readonly IPreviewDataProfileService _service;

        public BackofficePreviewDataProfileQueryHandler(IPreviewDataProfileService service) => _service = service;

        public Task<BackofficeProfilePreviewDto> Handle(BackofficePreviewDataProfileQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.Kind, query.DocumentId, query.FileName, query.Content, cancellationToken);
    }
}
