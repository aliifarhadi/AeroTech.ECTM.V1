using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation
{
    public sealed class CloseEngineInstallationService : ICloseEngineInstallationService
    {
        private readonly IEngineInstallationRepository _installations;
        private readonly IEngineInstallationLock _installationLock;
        private readonly EngineInstallationDtoLoader _installationDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public CloseEngineInstallationService(
            IEngineInstallationRepository installations,
            IEngineInstallationLock installationLock,
            EngineInstallationDtoLoader installationDtos,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _installations = installations;
            _installationLock = installationLock;
            _installationDtos = installationDtos;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<BackofficeEngineInstallationDto> CloseAsync(ICloseEngineInstallationCommand command, CancellationToken cancellationToken = default)
        {
            var installation = await _installations.GetAsync(command.InstallationId, cancellationToken)
                               ?? throw ExceptionFactory.EngineInstallationNotFound(command.InstallationId);

            await using var engineLock = await _installationLock.AcquireEngineAsync(installation.EngineId, cancellationToken);

            installation.Close(command.RemovedUtc, command.Reason, _idGenerator, _clock.GetDateTime());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _installationDtos.LoadAsync(installation, cancellationToken);
        }
    }
}
