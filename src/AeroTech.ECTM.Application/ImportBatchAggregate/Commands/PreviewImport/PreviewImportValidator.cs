using FluentValidation;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.PreviewImport
{
    public abstract class PreviewImportValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IPreviewImportCommand
    {
        protected PreviewImportValidator()
        {
            RuleFor(command => command.Files).NotEmpty();
            RuleForEach(command => command.Files).ChildRules(file =>
            {
                file.RuleFor(item => item.Name).NotEmpty().MaximumLength(256);
                file.RuleFor(item => item.Content).NotNull();
            });
            RuleFor(command => command.DefaultRegistration).MaximumLength(64);
            RuleFor(command => command.DefaultYear).InclusiveBetween(1, 9999);
        }
    }
}
