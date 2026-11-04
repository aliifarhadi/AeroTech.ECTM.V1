using FluentValidation;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft
{
    public abstract class UpdateAircraftValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IUpdateAircraftCommand
    {
        protected UpdateAircraftValidator()
        {
            RuleFor(command => command.TypeCode).MaximumLength(64);
            RuleFor(command => command.OperatorCode).MaximumLength(64);
            RuleFor(command => command.Status).IsInEnum();
        }
    }
}
