using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.UpdateBaseline
{
    public sealed class UpdateBaselineService : IUpdateBaselineService
    {
        private readonly IBaselineRepository _baselines;
        private readonly BaselineDefinitionsLoader _definitions;
        private readonly ShopVisitCheck _shopVisits;
        private readonly BaselineDtoLoader _baselineDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;

        public UpdateBaselineService(
            IBaselineRepository baselines,
            BaselineDefinitionsLoader definitions,
            ShopVisitCheck shopVisits,
            BaselineDtoLoader baselineDtos,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator)
        {
            _baselines = baselines;
            _definitions = definitions;
            _shopVisits = shopVisits;
            _baselineDtos = baselineDtos;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
        }

        public async Task<BackofficeBaselineDto> UpdateAsync(IUpdateBaselineCommand command, CancellationToken cancellationToken = default)
        {
            var baseline = await _baselines.GetAsync(command.BaselineId, cancellationToken)
                           ?? throw ExceptionFactory.BaselineNotFound(command.BaselineId);

            await _shopVisits.EnsureAsync(baseline.EngineId, command.ShopVisitEventId, cancellationToken);

            var definitions = await _definitions.LoadActiveAsync(cancellationToken);
            var parameters = BaselineDraftBuilder.ParametersOf(command.Parameters, definitions);
            var args = BaselineDraftBuilder.ArgsOf(command);

            baseline.Revise(command.ExpectedDraftRevision, args, parameters, _idGenerator);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _baselineDtos.LoadAsync(baseline, cancellationToken);
        }
    }
}
