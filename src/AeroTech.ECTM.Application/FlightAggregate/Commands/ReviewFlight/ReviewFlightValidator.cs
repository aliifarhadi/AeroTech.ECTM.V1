using AeroTech.Messages.Ectm.Enums;
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
            RuleFor(command => command.ReasonCode).IsInEnum().When(command => command.ReasonCode is not null);
            RuleFor(command => command.ReasonCode).NotNull().When(command => command.Action == ReviewAction.EXCLUDE);
            RuleFor(command => command.ReasonCode).Null().When(command => command.Action != ReviewAction.EXCLUDE);
            RuleFor(command => command.Note).NotEmpty().When(command => command.Action == ReviewAction.EXCLUDE);
        }
    }
}
