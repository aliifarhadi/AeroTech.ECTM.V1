using System.Text.Json;
using FluentValidation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.UpdateConfigurationDraft
{
    public abstract class UpdateConfigurationDraftValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IUpdateConfigurationDraftCommand
    {
        protected UpdateConfigurationDraftValidator()
        {
            RuleFor(command => command.Content.ValueKind)
                .Equal(JsonValueKind.Object)
                .WithMessage("Content must be one JSON object.");
        }
    }
}
