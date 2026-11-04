namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.RetireConfigurationDocument
{
    public interface IRetireConfigurationDocumentCommand
    {
        string Kind { get; }

        long DocumentId { get; }

        string? Note { get; }
    }
}
