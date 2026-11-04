using FluentValidation;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft
{
    public abstract class CreateAircraftValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICreateAircraftCommand
    {
        protected CreateAircraftValidator()
        {
            RuleFor(command => command.Registration).NotEmpty().MaximumLength(16);
            RuleFor(command => command.TypeCode).MaximumLength(64);
            RuleFor(command => command.OperatorCode).MaximumLength(64);
        }
    }
}
