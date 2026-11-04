using FluentValidation;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent
{
    public abstract class UpdateMaintenanceEventValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IUpdateMaintenanceEventCommand
    {
        protected UpdateMaintenanceEventValidator()
        {
            RuleFor(command => command.EventType).IsInEnum();
            RuleFor(command => command.EventUtc).NotEmpty();
            RuleFor(command => command.DateReliability).IsInEnum();
            RuleFor(command => command.WorkOrderReference).MaximumLength(128);
            RuleFor(command => command.Notes).MaximumLength(2000);
        }
    }
}
