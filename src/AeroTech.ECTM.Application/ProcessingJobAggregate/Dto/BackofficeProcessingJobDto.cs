using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.ProcessingJobAggregate;

namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Dto
{
    public sealed record BackofficeProcessingJobDto(
        long Id,
        EnumValueDto Kind,
        long SubjectId,
        EnumValueDto State,
        int Processed,
        int Total,
        string? ErrorCode,
        DateTimeOffset CreatedUtc,
        DateTimeOffset? CompletedUtc)
    {
        public static BackofficeProcessingJobDto From(ProcessingJob job)
            => new(
                job.Id,
                EnumValueDto.Of(job.Kind),
                job.SubjectId,
                EnumValueDto.Of(job.State),
                job.Processed,
                job.Total,
                job.ErrorCode,
                job.CreatedAt,
                job.CompletedAt);
    }
}
