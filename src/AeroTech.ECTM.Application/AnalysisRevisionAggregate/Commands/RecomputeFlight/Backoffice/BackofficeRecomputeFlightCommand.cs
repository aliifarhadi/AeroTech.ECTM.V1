using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight.Backoffice
{
    public sealed record BackofficeRecomputeFlightCommand(
        long FlightId,
        long? QualityRulesDocumentId,
        long? PhaseRuleSetDocumentId,
        long? SnapshotRulesDocumentId,
        bool DryRun) : IRequest<BackofficeRecomputeDto>, IRecomputeFlightCommand;
}
