using FluentValidation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.RetireConfigurationDocument
{
    public abstract class RetireConfigurationDocumentValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IRetireConfigurationDocumentCommand
    {
        protected RetireConfigurationDocumentValidator()
        {
            RuleFor(command => command.Note).MaximumLength(2000);
        }
    }
}
