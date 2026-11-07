using AeroTech.ECTM.Application.BaselineAggregate.Commands;
using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CreateBaseline.Backoffice
{
    public sealed record BackofficeCreateBaselineCommand(
        long EngineId,
        string Name,
        DateOnly? DeclaredEffectiveDate,
        long? ShopVisitEventId,
        bool FirstPostShopDeclared,
        string? SourceFileName,
        string? SourceFileSha256,
        string? Notes,
        IReadOnlyList<BaselineParameterInput> Parameters) : IRequest<BackofficeBaselineDto>, ICreateBaselineCommand;
}
