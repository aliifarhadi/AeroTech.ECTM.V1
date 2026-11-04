using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeTypedDefinitionKindDto(string Code, IReadOnlyList<string> Fields, string AvailableFrom, bool Evaluable)
    {
        public static BackofficeTypedDefinitionKindDto From(DerivedParameterKind kind)
            => new(kind.Code, kind.Fields, kind.AvailableFrom, kind.IsAvailableNow);
    }
}
