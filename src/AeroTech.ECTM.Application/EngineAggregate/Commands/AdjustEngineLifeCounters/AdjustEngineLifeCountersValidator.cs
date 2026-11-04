using FluentValidation;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters
{
    public abstract class AdjustEngineLifeCountersValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IAdjustEngineLifeCountersCommand
    {
        protected AdjustEngineLifeCountersValidator()
        {
            RuleFor(command => command.LifeCounters).NotNull().SetValidator(new LifeCountersInputValidator());
            RuleFor(command => command.Reason).NotEmpty().MaximumLength(256);
        }
    }
}
