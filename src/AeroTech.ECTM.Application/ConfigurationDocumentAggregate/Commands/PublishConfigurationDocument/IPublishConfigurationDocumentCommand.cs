namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.PublishConfigurationDocument
{
    public interface IPublishConfigurationDocumentCommand
    {
        string Kind { get; }

        long DocumentId { get; }

        string ApprovalNote { get; }
    }
}
