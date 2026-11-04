using System.Globalization;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineInstallationAggregate;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation
{
    public sealed class CreateEngineInstallationService : ICreateEngineInstallationService
    {
        private const string OpenEnded = "open";
        private const string InstantFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

        private readonly IEngineInstallationRepository _installations;
        private readonly IEngineRepository _engines;
        private readonly IAircraftRepository _aircraft;
        private readonly IEngineInstallationLock _installationLock;
        private readonly EngineInstallationDtoLoader _installationDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public CreateEngineInstallationService(
            IEngineInstallationRepository installations,
            IEngineRepository engines,
            IAircraftRepository aircraft,
            IEngineInstallationLock installationLock,
            EngineInstallationDtoLoader installationDtos,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _installations = installations;
            _engines = engines;
            _aircraft = aircraft;
            _installationLock = installationLock;
            _installationDtos = installationDtos;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<BackofficeEngineInstallationDto> CreateAsync(ICreateEngineInstallationCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _engines.ExistsAsync(command.EngineId, cancellationToken))
                throw ExceptionFactory.EngineNotFound(command.EngineId);

            if (!await _aircraft.ExistsAsync(command.AircraftId, cancellationToken))
                throw ExceptionFactory.AircraftNotFound(command.AircraftId);

            var installation = EngineInstallation.Record(
                _idGenerator.NewId(),
                command.EngineId,
                command.AircraftId,
                command.Position,
                command.InstalledUtc,
                command.Reason,
                command.Evidence,
                _idGenerator,
                _clock.GetDateTime());

            await using var engineLock = await _installationLock.AcquireEngineAsync(installation.EngineId, cancellationToken);
            await using var positionLock = await _installationLock.AcquirePositionAsync(installation.AircraftId, installation.Position, cancellationToken);

            var existing = await _installations.ListForEngineOrPositionAsync(installation.EngineId, installation.AircraftId, installation.Position, cancellationToken);
            var conflict = installation.FindOverlapIn(existing);

            if (conflict is not null)
                throw await OverlapErrorAsync(conflict, cancellationToken);

            await _installations.AddAsync(installation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _installationDtos.LoadAsync(installation, cancellationToken);
        }

        private async Task<BusinessException> OverlapErrorAsync(EngineInstallation conflict, CancellationToken cancellationToken)
        {
            var view = await _installationDtos.LoadAsync(conflict, cancellationToken);

            var installedText = view.InstalledUtc.UtcDateTime.ToString(InstantFormat, CultureInfo.InvariantCulture);
            var removedText = OpenEnded;

            if (view.RemovedUtc is { } removedUtc)
                removedText = removedUtc.UtcDateTime.ToString(InstantFormat, CultureInfo.InvariantCulture);

            return ExceptionFactory.InstallationOverlap(
                view.Id,
                view.EngineSerialNumber,
                view.AircraftRegistration,
                view.Position,
                installedText,
                removedText);
        }
    }
}
