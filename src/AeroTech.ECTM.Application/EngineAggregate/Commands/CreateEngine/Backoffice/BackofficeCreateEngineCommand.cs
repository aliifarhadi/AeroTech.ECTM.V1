using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine.Backoffice
{
    public sealed record BackofficeCreateEngineCommand(
        string SerialNumber,
        long EngineModelId,
        EngineStatus Status,
        string? VariantCode,
        string? ThrustRatingCode,
        string? ConfigurationEvidenceReference,
        LifeCountersInput? LifeCounters) : IRequest<BackofficeEngineDetailDto>, ICreateEngineCommand;
}
