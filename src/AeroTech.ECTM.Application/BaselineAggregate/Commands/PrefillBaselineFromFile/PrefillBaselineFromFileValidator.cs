using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PrefillBaselineFromFile
{
    public abstract class PrefillBaselineFromFileValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IPrefillBaselineFromFileCommand
    {
        protected PrefillBaselineFromFileValidator()
        {
            RuleFor(command => command.EngineId).GreaterThan(0);
            RuleFor(command => command.File).NotNull();
            RuleFor(command => command.File.Name).NotEmpty().When(command => command.File is not null);
            RuleFor(command => command.EnginePosition).InclusiveBetween(1, 2);
            RuleFor(command => command.ShopVisitEventId).GreaterThan(0).When(command => command.ShopVisitEventId is not null);
        }
    }
}
