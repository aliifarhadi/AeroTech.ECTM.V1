using AeroTech.ECTM.Application.ImportBatchAggregate.Services;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PrefillBaselineFromFile
{
    public interface IPrefillBaselineFromFileCommand
    {
        long EngineId { get; }

        UploadedFile File { get; }

        int EnginePosition { get; }

        long? ShopVisitEventId { get; }
    }
}
