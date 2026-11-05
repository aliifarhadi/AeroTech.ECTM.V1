namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight
{
    public interface IRecomputeFlightCommand
    {
        long FlightId { get; }

        long? QualityRulesDocumentId { get; }

        long? PhaseRuleSetDocumentId { get; }

        bool DryRun { get; }
    }
}
