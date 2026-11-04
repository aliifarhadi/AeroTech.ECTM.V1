using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel.Backoffice
{
    public sealed record BackofficeUpdateEngineModelCommand(
        long EngineModelId,
        string Manufacturer,
        string? Family,
        string? VariantCode,
        string? ThrustRatingCode,
        IReadOnlyList<string> DocumentReferences,
        string? Notes) : IRequest<BackofficeEngineModelDto>, IUpdateEngineModelCommand;
}
