using FluentValidation;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight
{
    public abstract class ReviewFlightValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IReviewFlightCommand
    {
        protected ReviewFlightValidator()
        {
            RuleFor(command => command.FlightId).GreaterThan(0);
            RuleFor(command => command.Action).IsInEnum();
            RuleFor(command => command.Note).MaximumLength(2000);
        }
    }
}
