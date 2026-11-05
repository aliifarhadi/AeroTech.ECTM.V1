using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeEngine.Backoffice
{
    public sealed record BackofficeRecomputeEngineCommand(
        long EngineId,
        long? QualityRulesDocumentId,
        long? PhaseRuleSetDocumentId,
        long? SnapshotRulesDocumentId) : IRequest<BackofficeEngineRecomputeDto>, IRecomputeEngineCommand;
}
