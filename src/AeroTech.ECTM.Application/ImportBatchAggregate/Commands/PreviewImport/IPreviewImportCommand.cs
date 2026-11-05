using AeroTech.ECTM.Application.ImportBatchAggregate.Services;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.PreviewImport
{
    public interface IPreviewImportCommand
    {
        IReadOnlyList<UploadedFile> Files { get; }

        string? DefaultRegistration { get; }

        int? DefaultYear { get; }
    }
}
