using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel
{
    public sealed class UpdateEngineModelService : IUpdateEngineModelService
    {
        private readonly IEngineModelRepository _engineModels;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEngineModelService(IEngineModelRepository engineModels, IUnitOfWork unitOfWork)
        {
            _engineModels = engineModels;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeEngineModelDto> UpdateAsync(IUpdateEngineModelCommand command, CancellationToken cancellationToken = default)
        {
            var engineModel = await _engineModels.GetAsync(command.EngineModelId, cancellationToken)
                              ?? throw ExceptionFactory.EngineModelNotFound(command.EngineModelId);

            engineModel.Update(
                command.Manufacturer,
                command.Family,
                command.VariantCode,
                command.ThrustRatingCode,
                command.DocumentReferences,
                command.Notes);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BackofficeEngineModelDto.From(engineModel);
        }
    }
}
