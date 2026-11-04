using FluentValidation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.PublishConfigurationDocument
{
    public abstract class PublishConfigurationDocumentValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IPublishConfigurationDocumentCommand
    {
        protected PublishConfigurationDocumentValidator()
        {
            RuleFor(command => command.ApprovalNote).NotEmpty().MaximumLength(2000);
        }
    }
}
