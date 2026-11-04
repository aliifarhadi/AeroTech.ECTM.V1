namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record DerivedParameterDefinition(
        string? Code,
        string? Kind,
        string? Role,
        string? UnitOut,
        string? Status,
        string? Description,
        string? A,
        string? B,
        string? Numerator,
        string? Denominator,
        bool? GuardZero,
        IReadOnlyList<LinearTerm>? Terms,
        double? Offset,
        string? Ref,
        double? ReferenceConstant,
        string? ExponentConstantRef);
}
