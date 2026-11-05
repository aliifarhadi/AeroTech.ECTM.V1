using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.SourceFileAggregate;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeSourceFileDto(long Id, string FileName, string Sha256, long SizeBytes, EnumValueDto SourceType, DateTimeOffset StoredUtc)
    {
        public static BackofficeSourceFileDto From(SourceFile sourceFile)
            => new(sourceFile.Id, sourceFile.FileName, sourceFile.Sha256, sourceFile.SizeBytes, EnumValueDto.Of(sourceFile.SourceType), sourceFile.StoredAt);
    }
}
