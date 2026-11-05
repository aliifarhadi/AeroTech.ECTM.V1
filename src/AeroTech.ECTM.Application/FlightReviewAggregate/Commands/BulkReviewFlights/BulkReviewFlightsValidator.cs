using AeroTech.Messages.Ectm.Enums;
using FluentValidation;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Commands.BulkReviewFlights
{
    public abstract class BulkReviewFlightsValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IBulkReviewFlightsCommand
    {
        public const int MaximumFlights = 200;

        protected BulkReviewFlightsValidator()
        {
            RuleFor(command => command.FlightIds).NotEmpty();
            RuleFor(command => command.FlightIds.Count).LessThanOrEqualTo(MaximumFlights);
            RuleForEach(command => command.FlightIds).GreaterThan(0);
            RuleFor(command => command.Action).IsInEnum();
            RuleFor(command => command.Note).MaximumLength(2000);
            RuleFor(command => command.ReasonCode).IsInEnum().When(command => command.ReasonCode is not null);
            RuleFor(command => command.ReasonCode).NotNull().When(command => command.Action == ReviewAction.EXCLUDE);
            RuleFor(command => command.ReasonCode).Null().When(command => command.Action != ReviewAction.EXCLUDE);
            RuleFor(command => command.Note).NotEmpty().When(command => command.Action == ReviewAction.EXCLUDE);
        }
    }
}
