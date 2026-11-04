using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.PreviewDataProfile.Backoffice
{
    public sealed record BackofficePreviewDataProfileQuery(
        string Kind,
        long DocumentId,
        string FileName,
        byte[] Content) : IRequest<BackofficeProfilePreviewDto>;
}
