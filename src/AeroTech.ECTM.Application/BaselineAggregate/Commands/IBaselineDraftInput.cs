namespace AeroTech.ECTM.Application.BaselineAggregate.Commands
{
    public interface IBaselineDraftInput
    {
        string Name { get; }

        DateOnly? DeclaredEffectiveDate { get; }

        long? ShopVisitEventId { get; }

        bool FirstPostShopDeclared { get; }

        string? SourceFileName { get; }

        string? SourceFileSha256 { get; }

        string? Notes { get; }

        IReadOnlyList<BaselineParameterInput> Parameters { get; }
    }
}
