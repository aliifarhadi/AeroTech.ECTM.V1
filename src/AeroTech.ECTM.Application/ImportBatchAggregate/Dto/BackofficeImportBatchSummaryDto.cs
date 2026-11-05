using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.ImportBatchAggregate;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Dto
{
    public sealed record BackofficeImportBatchSummaryDto(
        long Id,
        EnumValueDto State,
        long? JobId,
        DateTimeOffset CreatedUtc,
        DateTimeOffset? CommittedUtc,
        BackofficeImportCountsDto Counts)
    {
        public static BackofficeImportBatchSummaryDto From(ImportBatch batch, BackofficeImportCountsDto counts)
            => new(batch.Id, EnumValueDto.Of(batch.State), batch.JobId, batch.CreatedAt, batch.CommittedAt, counts);
    }
}
