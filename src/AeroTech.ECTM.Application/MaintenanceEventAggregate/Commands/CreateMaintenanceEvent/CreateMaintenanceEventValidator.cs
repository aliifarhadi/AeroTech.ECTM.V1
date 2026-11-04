using FluentValidation;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent
{
    public abstract class CreateMaintenanceEventValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICreateMaintenanceEventCommand
    {
        protected CreateMaintenanceEventValidator()
        {
            RuleFor(command => command.EventType).IsInEnum();
            RuleFor(command => command.EventUtc).NotEmpty();
            RuleFor(command => command.DateReliability).IsInEnum();
            RuleFor(command => command.WorkOrderReference).MaximumLength(128);
            RuleFor(command => command.Notes).MaximumLength(2000);
        }
    }
}
