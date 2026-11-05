using FluentValidation;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity
{
    public abstract class ConfirmFlightIdentityValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IConfirmFlightIdentityCommand
    {
        protected ConfirmFlightIdentityValidator()
        {
            RuleFor(command => command.FlightId).GreaterThan(0);
            RuleFor(command => command.AircraftId).GreaterThan(0);
            RuleFor(command => command.FlightDate).NotEmpty();
            RuleFor(command => command.FlightNumber).NotEmpty().MaximumLength(32);
            RuleFor(command => command.Engines).NotEmpty();
            RuleFor(command => command.Note).MaximumLength(2000);
        }
    }
}
