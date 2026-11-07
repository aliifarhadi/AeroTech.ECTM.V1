using AeroTech.ECTM.Application.BaselineAggregate.Commands;

namespace AeroTech.ECTM.RestApi.V1.BaselineAggregate.Requests
{
    public sealed record CreateBaselineRequest(
        string Name,
        DateOnly? DeclaredEffectiveDate,
        long? ShopVisitEventId,
        bool FirstPostShopDeclared,
        string? SourceFileName,
        string? SourceFileSha256,
        string? Notes,
        IReadOnlyList<BaselineParameterInput> Parameters);
}
