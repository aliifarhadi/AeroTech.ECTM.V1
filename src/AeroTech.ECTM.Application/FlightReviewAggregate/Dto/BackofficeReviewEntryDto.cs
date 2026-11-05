using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Entities;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Dto
{
    public sealed record BackofficeReviewEntryDto(EnumValueDto Action, string? Actor, EnumValueDto? ReasonCode, string? Note, EnumValueDto FlightStateAfter, DateTimeOffset AtUtc)
    {
        public static BackofficeReviewEntryDto From(FlightReviewEntry entry)
            => new(EnumValueDto.Of(entry.Action), entry.Actor, EnumValueDto.OfNullable(entry.ReasonCode), entry.Note, EnumValueDto.Of(entry.FlightStateAfter), entry.At);
    }
}
