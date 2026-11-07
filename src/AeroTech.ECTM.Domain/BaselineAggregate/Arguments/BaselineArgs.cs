namespace AeroTech.ECTM.Domain.BaselineAggregate.Arguments
{
    public sealed record BaselineArgs(
        string Name,
        DateOnly? DeclaredEffectiveDate,
        long? ShopVisitEventId,
        bool FirstPostShopDeclared,
        string? SourceFileName,
        string? SourceFileSha256,
        string? Notes);
}
