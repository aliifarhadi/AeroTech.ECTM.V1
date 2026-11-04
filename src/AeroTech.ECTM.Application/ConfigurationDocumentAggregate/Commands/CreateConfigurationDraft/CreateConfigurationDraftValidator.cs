using FluentValidation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.CreateConfigurationDraft
{
    public abstract class CreateConfigurationDraftValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICreateConfigurationDraftCommand
    {
        protected CreateConfigurationDraftValidator()
        {
            RuleFor(command => command.BasedOnDocumentId).GreaterThan(0).When(command => command.BasedOnDocumentId is not null);
        }
    }
}
