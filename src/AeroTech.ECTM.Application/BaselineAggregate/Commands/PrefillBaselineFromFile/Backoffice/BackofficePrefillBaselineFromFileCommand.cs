using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PrefillBaselineFromFile.Backoffice
{
    public sealed record BackofficePrefillBaselineFromFileCommand(
        long EngineId,
        UploadedFile File,
        int EnginePosition,
        long? ShopVisitEventId) : IRequest<BackofficeBaselinePrefillDto>, IPrefillBaselineFromFileCommand;
}
