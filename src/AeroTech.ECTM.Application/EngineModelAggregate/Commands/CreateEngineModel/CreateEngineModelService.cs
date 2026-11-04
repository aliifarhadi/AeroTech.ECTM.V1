using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using AeroTech.ECTM.Application.EngineModelAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineModelAggregate;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel
{
    public sealed class CreateEngineModelService : ICreateEngineModelService
    {
        private readonly IEngineModelRepository _engineModels;
        private readonly IEngineModelLock _engineModelLock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;

        public CreateEngineModelService(
            IEngineModelRepository engineModels,
            IEngineModelLock engineModelLock,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator)
        {
            _engineModels = engineModels;
            _engineModelLock = engineModelLock;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
        }

        public async Task<BackofficeEngineModelDto> CreateAsync(ICreateEngineModelCommand command, CancellationToken cancellationToken = default)
        {
            var engineModel = EngineModel.Register(
                _idGenerator.NewId(),
                command.Code,
                command.Manufacturer,
                command.Family,
                command.VariantCode,
                command.ThrustRatingCode,
                command.DocumentReferences,
                command.Notes);

            await using (await _engineModelLock.AcquireCodeAsync(engineModel.Code, cancellationToken))
            {
                if (await _engineModels.CodeExistsAsync(engineModel.Code, cancellationToken))
                    throw ExceptionFactory.EngineModelCodeAlreadyExists(engineModel.Code);

                await _engineModels.AddAsync(engineModel, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return BackofficeEngineModelDto.From(engineModel);
        }
    }
}
