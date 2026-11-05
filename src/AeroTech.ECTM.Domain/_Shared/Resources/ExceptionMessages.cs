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
        public const string ConfigurationKindNotAuthorable = "Configuration kind {0} is delivered in phase {1}.";
        public const string ConfigurationDocumentIsNotDraft = "Configuration document {0} is {1}; only a draft can be changed or published.";
        public const string ConfigurationDocumentIsNotPublished = "Configuration document {0} is {1}; only a published document can be retired.";
        public const string ConfigurationDocumentIsInvalid = "Configuration document {0} has {1} validation issue(s).";
        public const string ApprovalNoteIsRequired = "Publishing requires an approval note.";
        public const string BasedOnDocumentIsInvalid = "Document {0} cannot be the base of a {1} draft.";
        public const string DryRunNotAvailable = "A dry-run of {0} is delivered in phase {1}.";
        public const string SourceFileIsUnreadable = "The source file cannot be read: {0}";
        public const string SourceFileIsTooLarge = "The source file has {0} bytes; the limit is {1}.";
        public const string TypedDefinitionIsInvalid = "The typed definition is invalid: {0}";
        public const string ActiveSetIncomplete = "No published {0} document is active.";

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

        // Imports and flights
        public const string ImportBatchNotFound = "Import batch {0} was not found.";
        public const string ImportFileTypeNotSupported = "File {0} is not a CSV, XLSX, ZIP or RAR file.";
        public const string ImportFileIsTooLarge = "File {0} has {1} bytes; the limit is {2}.";
        public const string ArchiveIsUnreadable = "Archive {0} cannot be read.";
        public const string ArchiveIsEncrypted = "Archive {0} is encrypted.";
        public const string ImportHasTooManyFiles = "The upload holds {0} files; the limit is {1}.";
        public const string ImportExpandsBeyondLimit = "The upload expands beyond {0} bytes.";
        public const string ImportBatchCommittedWithAnotherKey = "Import batch {0} is {1} with another idempotency key.";
        public const string ArchiveHasNoMembers = "Archive {0} holds no file.";
        public const string FlightIdentityIsIncomplete = "The identity is incomplete: {0}";
        public const string ProcessingJobNotFound = "Processing job {0} was not found.";
        public const string FlightNotFound = "Flight {0} was not found.";
        public const string EngineNotInstalledAtFlight = "Engine {0} is not installed on aircraft {1} position {2} on {3}.";
        public const string PossibleDuplicateNeedsConfirmation = "Flight {0} has the same aircraft, date and flight number; confirm that this is a distinct flight.";
        public const string ReviewNotAllowed = "{0} is not allowed while the flight is {1}.";
        public const string FlightParameterNotFound = "Flight {0} has no numeric channel {1}.";
        public const string SeriesRangeTooLarge = "RAW sampling returns every sample; the range holds {0} samples and the limit is {1}.";
        public const string FlightQualityNotEvaluated = "Flight {0} has no quality result yet; publish QUALITY_RULES and recompute the flight.";
        public const string FlightPhasesNotEvaluated = "Flight {0} has no phase result in its current analysis revision; publish PHASE_RULESET and recompute the flight.";
        public const string SampleIndexOutOfRange = "Sample {0} is outside the flight, which has samples 0 to {1}.";
        public const string RecomputeChangesNothing = "Revision {0} of flight {1} already uses these versions, so a recompute gives the same result.";
        public const string RecomputeAlreadyPending = "Flight {0} already has revision {1} waiting for its job.";
        public const string DocumentNotPublishedOfKind = "Configuration document {0} is not a published {1}.";
        public const string StabilityPointsNotEvaluated = "Flight {0} has no stability points in its current analysis revision; publish SNAPSHOT_RULES and recompute the flight.";
        public const string FlightNotAnalysed = "Flight {0} has no analysis revision yet, so it cannot be accepted or excluded.";
        public const string ReviewRevisionStale = "The review names analysis revision {0}, but the current revision is {1}; reload the flight.";
    }
}
