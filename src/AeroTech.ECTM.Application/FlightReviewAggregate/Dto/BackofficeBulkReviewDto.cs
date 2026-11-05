using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Dto
{
    public sealed record BackofficeBulkReviewDto(EnumValueDto Action, int Applied, int Refused, IReadOnlyList<BackofficeBulkReviewItemDto> Items);
}
