using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CreateBaseline
{
    public abstract class CreateBaselineValidator<TCommand> : BaselineDraftValidator<TCommand>
        where TCommand : ICreateBaselineCommand
    {
        protected CreateBaselineValidator()
        {
            RuleFor(command => command.EngineId).GreaterThan(0);
        }
    }
}
