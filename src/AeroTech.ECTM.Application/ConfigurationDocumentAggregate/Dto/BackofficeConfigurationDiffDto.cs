namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeConfigurationDiffDto(
        BackofficeConfigurationDocumentDto From,
        BackofficeConfigurationDocumentDto To,
        IReadOnlyList<BackofficeJsonChangeDto> Changes);
}
