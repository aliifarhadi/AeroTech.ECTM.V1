using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel.Backoffice
{
    public sealed record BackofficeCreateEngineModelCommand(
        string Code,
        string Manufacturer,
        string? Family,
        string? VariantCode,
        string? ThrustRatingCode,
        IReadOnlyList<string> DocumentReferences,
        string? Notes) : IRequest<BackofficeEngineModelDto>, ICreateEngineModelCommand;
}
