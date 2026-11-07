using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficeBaselinePublicationDto(BackofficeBaselineDto Baseline, BackofficeProcessingJobDto Evaluation);
}
