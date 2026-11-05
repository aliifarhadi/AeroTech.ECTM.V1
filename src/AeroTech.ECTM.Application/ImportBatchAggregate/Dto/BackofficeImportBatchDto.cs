using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightAggregate.Dto;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Dto
{
    public sealed record BackofficeImportBatchDto(
        long Id,
        EnumValueDto State,
        BackofficeConfigurationVersionsDto Versions,
        string? DefaultRegistration,
        int? DefaultYear,
        long? JobId,
        DateTimeOffset CreatedUtc,
        DateTimeOffset? CommittedUtc,
        BackofficeImportCountsDto Counts,
        IReadOnlyList<BackofficeImportItemDto> Items);
}
