using AeroTech.Framework.Core.Domain.Exceptions;

namespace AeroTech.ECTM.Domain._Shared.Resources
{
    public static class ExceptionFactory
    {
        // Shared: 80000-80099
        public static BusinessException OperationInProgress(params object?[] args) =>
            new(80000, ExceptionMessages.OperationInProgress, args) { HttpStatus = 409 };

        public static BusinessException ResourceChangedConcurrently() =>
            new(80001, ExceptionMessages.ResourceChangedConcurrently) { HttpStatus = 409 };

        // Configuration registry: 80100-80199
        public static BusinessException ConfigurationKindNotFound(params object?[] args) =>
            new(80100, ExceptionMessages.ConfigurationKindNotFound, args) { HttpStatus = 404 };

        public static BusinessException ConfigurationDocumentNotFound(params object?[] args) =>
            new(80101, ExceptionMessages.ConfigurationDocumentNotFound, args) { HttpStatus = 404 };

        public static BusinessException ConfigurationDocumentVersionIsInvalid(params object?[] args) =>
            new(80102, ExceptionMessages.ConfigurationDocumentVersionIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException ConfigurationDocumentContentIsRequired() =>
            new(80103, ExceptionMessages.ConfigurationDocumentContentIsRequired) { HttpStatus = 422 };

        public static BusinessException ConfigurationDocumentContentIsNotJsonObject() =>
            new(80104, ExceptionMessages.ConfigurationDocumentContentIsNotJsonObject) { HttpStatus = 422 };

        // Fleet registry: 80200-80299
        public static BusinessException AircraftNotFound(params object?[] args) =>
            new(80200, ExceptionMessages.AircraftNotFound, args) { HttpStatus = 404 };

        public static BusinessException AircraftRegistrationAlreadyExists(params object?[] args) =>
            new(80201, ExceptionMessages.AircraftRegistrationAlreadyExists, args) { HttpStatus = 409 };

        public static BusinessException AircraftRegistrationIsRequired() =>
            new(80202, ExceptionMessages.AircraftRegistrationIsRequired) { HttpStatus = 422 };

        public static BusinessException EngineModelNotFound(params object?[] args) =>
            new(80203, ExceptionMessages.EngineModelNotFound, args) { HttpStatus = 404 };

        public static BusinessException EngineModelCodeAlreadyExists(params object?[] args) =>
            new(80204, ExceptionMessages.EngineModelCodeAlreadyExists, args) { HttpStatus = 409 };

        public static BusinessException EngineModelIsIncomplete() =>
            new(80205, ExceptionMessages.EngineModelIsIncomplete) { HttpStatus = 422 };

        public static BusinessException EngineNotFound(params object?[] args) =>
            new(80206, ExceptionMessages.EngineNotFound, args) { HttpStatus = 404 };

        public static BusinessException EngineSerialAlreadyExists(params object?[] args) =>
            new(80207, ExceptionMessages.EngineSerialAlreadyExists, args) { HttpStatus = 409 };

        public static BusinessException EngineSerialIsRequired() =>
            new(80208, ExceptionMessages.EngineSerialIsRequired) { HttpStatus = 422 };

        public static BusinessException EnginePositionIsInvalid(params object?[] args) =>
            new(80209, ExceptionMessages.EnginePositionIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException EngineInstallationNotFound(params object?[] args) =>
            new(80210, ExceptionMessages.EngineInstallationNotFound, args) { HttpStatus = 404 };

        public static BusinessException InstallationOverlap(params object?[] args) =>
            new(80211, ExceptionMessages.InstallationOverlap, args) { HttpStatus = 409 };

        public static BusinessException InstallationIntervalIsInvalid() =>
            new(80212, ExceptionMessages.InstallationIntervalIsInvalid) { HttpStatus = 422 };

        public static BusinessException EngineConfigurationEvidenceIsRequired() =>
            new(80213, ExceptionMessages.EngineConfigurationEvidenceIsRequired) { HttpStatus = 422 };

        public static BusinessException LifeCountersAreInvalid() =>
            new(80214, ExceptionMessages.LifeCountersAreInvalid) { HttpStatus = 422 };

        public static BusinessException InstallationAlreadyClosed(params object?[] args) =>
            new(80215, ExceptionMessages.InstallationAlreadyClosed, args) { HttpStatus = 409 };

        public static BusinessException MaintenanceEventIdentityIsInvalid() =>
            new(80216, ExceptionMessages.MaintenanceEventIdentityIsInvalid) { HttpStatus = 422 };

        public static BusinessException MaintenanceEventCountersAreInvalid() =>
            new(80217, ExceptionMessages.MaintenanceEventCountersAreInvalid) { HttpStatus = 422 };

        public static BusinessException MaintenanceEventNotFound(params object?[] args) =>
            new(80218, ExceptionMessages.MaintenanceEventNotFound, args) { HttpStatus = 404 };

        public static BusinessException RegistryChangeReasonIsRequired() =>
            new(80219, ExceptionMessages.RegistryChangeReasonIsRequired) { HttpStatus = 422 };

        public static BusinessException LifeCounterAdjustmentReasonIsRequired() =>
            new(80220, ExceptionMessages.LifeCounterAdjustmentReasonIsRequired) { HttpStatus = 422 };
    }
}
