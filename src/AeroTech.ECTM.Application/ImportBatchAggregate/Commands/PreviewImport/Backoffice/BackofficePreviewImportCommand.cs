using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using MediatR;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.PreviewImport.Backoffice
{
    public sealed record BackofficePreviewImportCommand(
        IReadOnlyList<UploadedFile> Files,
        string? DefaultRegistration,
        int? DefaultYear) : IRequest<BackofficeImportBatchDto>, IPreviewImportCommand;
}
