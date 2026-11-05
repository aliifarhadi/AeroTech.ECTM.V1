using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Dto
{
    public sealed record BackofficeBulkReviewItemDto(
        long FlightId,
        EnumValueDto Outcome,
        EnumValueDto? FlightState,
        int? ErrorCode,
        IReadOnlyList<BackofficeAcceptanceRefusalDto> Refusals);
}
