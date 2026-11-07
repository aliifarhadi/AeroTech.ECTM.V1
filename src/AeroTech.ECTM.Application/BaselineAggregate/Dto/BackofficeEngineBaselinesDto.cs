using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficeEngineBaselinesDto(
        long EngineId,
        string? EngineSerialNumber,
        BackofficeProcessingJobDto? LatestEvaluation,
        IReadOnlyList<BackofficeBaselineDto> Baselines);
}
