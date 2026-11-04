namespace AeroTech.ECTM.Domain._Shared.Resources
{
    public static class ExceptionMessages
    {
        // Shared
        public const string OperationInProgress = "Another operation on {0} is in progress.";
        public const string ResourceChangedConcurrently = "The resource was changed by another operation. Reload it and try again.";

        // Configuration registry
        public const string ConfigurationKindNotFound = "Configuration kind {0} was not found.";
        public const string ConfigurationDocumentNotFound = "Configuration document {0} was not found.";
        public const string ConfigurationDocumentVersionIsInvalid = "Configuration document version {0} is invalid. Versions start at 1.";
        public const string ConfigurationDocumentContentIsRequired = "A configuration document requires content.";
        public const string ConfigurationDocumentContentIsNotJsonObject = "Configuration document content must be one JSON object.";

        // Fleet registry
        public const string AircraftNotFound = "Aircraft {0} was not found.";
        public const string AircraftRegistrationAlreadyExists = "Aircraft registration {0} already exists.";
        public const string AircraftRegistrationIsRequired = "An aircraft registration is required.";
        public const string EngineModelNotFound = "Engine model {0} was not found.";
        public const string EngineModelCodeAlreadyExists = "Engine model code {0} already exists.";
        public const string EngineModelIsIncomplete = "An engine model requires a code and a manufacturer.";
        public const string EngineNotFound = "Engine {0} was not found.";
        public const string EngineSerialAlreadyExists = "Engine {0} {1} already exists.";
        public const string EngineSerialIsRequired = "An engine requires a serial number.";
        public const string EnginePositionIsInvalid = "Engine position {0} is invalid. Positions are 1 and 2.";
        public const string EngineInstallationNotFound = "Engine installation {0} was not found.";
        public const string InstallationOverlap = "The installation overlaps installation {0}: engine {1} on aircraft {2} position {3} from {4} to {5}.";
        public const string InstallationIntervalIsInvalid = "The removal time must be after the installation time.";
        public const string EngineConfigurationEvidenceIsRequired = "A documented variant or thrust rating requires an evidence reference.";
        public const string LifeCountersAreInvalid = "Life counters must not be negative.";
        public const string InstallationAlreadyClosed = "Engine installation {0} is already closed.";
        public const string MaintenanceEventIdentityIsInvalid = "A maintenance event belongs either to an engine, or to an aircraft position without an engine.";
        public const string MaintenanceEventCountersAreInvalid = "Hours and cycles at a maintenance event must not be negative.";
        public const string MaintenanceEventNotFound = "Maintenance event {0} was not found.";
        public const string RegistryChangeReasonIsRequired = "A reason is required for every installation and removal.";
        public const string LifeCounterAdjustmentReasonIsRequired = "A reason is required for every life counter adjustment.";
    }
}
