using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.RetireBaseline
{
    public abstract class RetireBaselineValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IRetireBaselineCommand
    {
        protected RetireBaselineValidator()
        {
            RuleFor(command => command.BaselineId).GreaterThan(0);
            RuleFor(command => command.Reason).NotEmpty().MaximumLength(2000);
        }
    }
}
