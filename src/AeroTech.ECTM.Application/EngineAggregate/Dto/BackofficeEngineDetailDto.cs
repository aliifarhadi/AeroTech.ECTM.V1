using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.EngineAggregate;

namespace AeroTech.ECTM.Application.EngineAggregate.Dto
{
    public sealed record BackofficeEngineDetailDto(
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
        BackofficeEngineInstallationDto? CurrentInstallation,
        IReadOnlyList<BackofficeLifeCounterAdjustmentDto> LifeCounterAdjustments)
    {
        public static BackofficeEngineDetailDto From(Engine engine, string engineModelCode, BackofficeEngineInstallationDto? currentInstallation)
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
                currentInstallation,
                engine.LifeCounterAdjustments
                    .OrderBy(adjustment => adjustment.AdjustedAt)
                    .ThenBy(adjustment => adjustment.Id)
                    .Select(BackofficeLifeCounterAdjustmentDto.From)
                    .ToList());
    }
}
