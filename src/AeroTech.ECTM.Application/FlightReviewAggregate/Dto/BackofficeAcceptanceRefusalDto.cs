using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Dto
{
    public sealed record BackofficeAcceptanceRefusalDto(string Code, EnumValueDto? Kind, int? Position)
    {
        public static BackofficeAcceptanceRefusalDto From(AcceptanceRefusal refusal)
            => new(refusal.Code, EnumValueDto.OfNullable(refusal.Kind), refusal.Position);
    }
}
