namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeReadinessDto(
        bool ParserReady,
        IReadOnlyList<string> MissingForParser,
        IReadOnlyList<BackofficeReadinessKindDto> Kinds);
}
