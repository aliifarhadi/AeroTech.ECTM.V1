using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CloneBaseline
{
    public abstract class CloneBaselineValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICloneBaselineCommand
    {
        protected CloneBaselineValidator()
        {
            RuleFor(command => command.BaselineId).GreaterThan(0);
        }
    }
}
