using FluentValidation;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight
{
    public abstract class RecomputeFlightValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IRecomputeFlightCommand
    {
        protected RecomputeFlightValidator()
        {
            RuleFor(command => command.FlightId).GreaterThan(0);
            RuleFor(command => command.QualityRulesDocumentId).GreaterThan(0).When(command => command.QualityRulesDocumentId is not null);
            RuleFor(command => command.PhaseRuleSetDocumentId).GreaterThan(0).When(command => command.PhaseRuleSetDocumentId is not null);
        }
    }
}
