namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeEngine
{
    public interface IRecomputeEngineCommand
    {
        long EngineId { get; }

        long? QualityRulesDocumentId { get; }

        long? PhaseRuleSetDocumentId { get; }

        long? SnapshotRulesDocumentId { get; }
    }
}
