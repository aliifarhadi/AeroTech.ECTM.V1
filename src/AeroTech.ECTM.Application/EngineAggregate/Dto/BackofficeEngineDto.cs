using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.EngineAggregate;

namespace AeroTech.ECTM.Application.EngineAggregate.Dto
{
    public sealed record BackofficeEngineDto(
        long Id,
        string SerialNumber,
        string Manufacturer,
        long EngineModelId,
        string EngineModelCode,
        EnumValueDto Status,
        string? VariantCode,
        string? ThrustRatingCode,
        BackofficeConfigurationEvidenceDto? ConfigurationEvidence,
        BackofficeLifeCountersDto? LifeCounters,
        BackofficeEngineInstallationDto? CurrentInstallation)
    {
        public static BackofficeEngineDto From(Engine engine, string engineModelCode, BackofficeEngineInstallationDto? currentInstallation)
            => new(
                engine.Id,
                engine.SerialNumber,
                engine.Manufacturer,
                engine.EngineModelId,
                engineModelCode,
                EnumValueDto.Of(engine.Status),
                engine.VariantCode,
                engine.ThrustRatingCode,
                BackofficeConfigurationEvidenceDto.From(engine),
                BackofficeLifeCountersDto.From(engine),
                currentInstallation);
    }
}
