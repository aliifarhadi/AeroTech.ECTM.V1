using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine.Backoffice
{
    public sealed record BackofficeUpdateEngineCommand(
        long EngineId,
        EngineStatus Status,
        string? VariantCode,
        string? ThrustRatingCode,
        string? ConfigurationEvidenceReference) : IRequest<BackofficeEngineDetailDto>, IUpdateEngineCommand;
}
