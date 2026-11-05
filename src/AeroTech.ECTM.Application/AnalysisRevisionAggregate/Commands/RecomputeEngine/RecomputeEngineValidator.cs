using FluentValidation;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeEngine
{
    public abstract class RecomputeEngineValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IRecomputeEngineCommand
    {
        protected RecomputeEngineValidator()
        {
            RuleFor(command => command.EngineId).GreaterThan(0);
            RuleFor(command => command.QualityRulesDocumentId).GreaterThan(0).When(command => command.QualityRulesDocumentId is not null);
            RuleFor(command => command.PhaseRuleSetDocumentId).GreaterThan(0).When(command => command.PhaseRuleSetDocumentId is not null);
            RuleFor(command => command.SnapshotRulesDocumentId).GreaterThan(0).When(command => command.SnapshotRulesDocumentId is not null);
        }
    }
}
