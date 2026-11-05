using AeroTech.Framework.Core.Domain.Exceptions;

namespace AeroTech.ECTM.Domain._Shared.Resources
{
    public static class ExceptionFactory
    {
        public const int ResourceChangedConcurrentlyCode = 80001;
        public const int SourceFileIsUnreadableCode = 80112;
        public const int SourceFileIsTooLargeCode = 80113;

        // Shared: 80000-80099
        public static BusinessException OperationInProgress(params object?[] args) =>
            new(80000, ExceptionMessages.OperationInProgress, args) { HttpStatus = 409 };

        public static BusinessException ResourceChangedConcurrently() =>
            new(ResourceChangedConcurrentlyCode, ExceptionMessages.ResourceChangedConcurrently) { HttpStatus = 409 };

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

        public static BusinessException ConfigurationKindNotAuthorable(params object?[] args) =>
            new(80105, ExceptionMessages.ConfigurationKindNotAuthorable, args) { HttpStatus = 422 };

        public static BusinessException ConfigurationDocumentIsNotDraft(params object?[] args) =>
            new(80106, ExceptionMessages.ConfigurationDocumentIsNotDraft, args) { HttpStatus = 409 };

        public static BusinessException ConfigurationDocumentIsNotPublished(params object?[] args) =>
            new(80107, ExceptionMessages.ConfigurationDocumentIsNotPublished, args) { HttpStatus = 409 };

        public static BusinessException ConfigurationDocumentIsInvalid(params object?[] args) =>
            new(80108, ExceptionMessages.ConfigurationDocumentIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException ApprovalNoteIsRequired() =>
            new(80109, ExceptionMessages.ApprovalNoteIsRequired) { HttpStatus = 422 };

        public static BusinessException BasedOnDocumentIsInvalid(params object?[] args) =>
            new(80110, ExceptionMessages.BasedOnDocumentIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException DryRunNotAvailable(params object?[] args) =>
            new(80111, ExceptionMessages.DryRunNotAvailable, args) { HttpStatus = 422 };

        public static BusinessException SourceFileIsUnreadable(params object?[] args) =>
            new(SourceFileIsUnreadableCode, ExceptionMessages.SourceFileIsUnreadable, args) { HttpStatus = 422 };

        public static BusinessException SourceFileIsTooLarge(params object?[] args) =>
            new(SourceFileIsTooLargeCode, ExceptionMessages.SourceFileIsTooLarge, args) { HttpStatus = 422 };


        public static BusinessException TypedDefinitionIsInvalid(params object?[] args) =>
            new(80115, ExceptionMessages.TypedDefinitionIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException ActiveSetIncomplete(params object?[] args) =>
            new(80116, ExceptionMessages.ActiveSetIncomplete, args) { HttpStatus = 422 };

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

        // Imports and flights: 80300-80399
        public static BusinessException ImportBatchNotFound(params object?[] args) =>
            new(80300, ExceptionMessages.ImportBatchNotFound, args) { HttpStatus = 404 };

        public static BusinessException ImportFileTypeNotSupported(params object?[] args) =>
            new(80301, ExceptionMessages.ImportFileTypeNotSupported, args) { HttpStatus = 422 };

        public static BusinessException ImportFileIsTooLarge(params object?[] args) =>
            new(80302, ExceptionMessages.ImportFileIsTooLarge, args) { HttpStatus = 422 };

        public static BusinessException ArchiveIsUnreadable(params object?[] args) =>
            new(80303, ExceptionMessages.ArchiveIsUnreadable, args) { HttpStatus = 422 };

        public static BusinessException ArchiveIsEncrypted(params object?[] args) =>
            new(80304, ExceptionMessages.ArchiveIsEncrypted, args) { HttpStatus = 422 };

        public static BusinessException ImportHasTooManyFiles(params object?[] args) =>
            new(80305, ExceptionMessages.ImportHasTooManyFiles, args) { HttpStatus = 422 };

        public static BusinessException ImportExpandsBeyondLimit(params object?[] args) =>
            new(80306, ExceptionMessages.ImportExpandsBeyondLimit, args) { HttpStatus = 422 };

        public static BusinessException ImportBatchCommittedWithAnotherKey(params object?[] args) =>
            new(80307, ExceptionMessages.ImportBatchCommittedWithAnotherKey, args) { HttpStatus = 409 };

        public static BusinessException ArchiveHasNoMembers(params object?[] args) =>
            new(80308, ExceptionMessages.ArchiveHasNoMembers, args) { HttpStatus = 422 };

        public static BusinessException FlightIdentityIsIncomplete(params object?[] args) =>
            new(80309, ExceptionMessages.FlightIdentityIsIncomplete, args) { HttpStatus = 422 };

        public static BusinessException ProcessingJobNotFound(params object?[] args) =>
            new(80310, ExceptionMessages.ProcessingJobNotFound, args) { HttpStatus = 404 };

        public static BusinessException FlightNotFound(params object?[] args) =>
            new(80311, ExceptionMessages.FlightNotFound, args) { HttpStatus = 404 };

        public static BusinessException EngineNotInstalledAtFlight(params object?[] args) =>
            new(80312, ExceptionMessages.EngineNotInstalledAtFlight, args) { HttpStatus = 422 };

        public static BusinessException PossibleDuplicateNeedsConfirmation(params object?[] args) =>
            new(80313, ExceptionMessages.PossibleDuplicateNeedsConfirmation, args) { HttpStatus = 422 };

        public static BusinessException ReviewNotAllowed(params object?[] args) =>
            new(80314, ExceptionMessages.ReviewNotAllowed, args) { HttpStatus = 409 };

        public static BusinessException ReviewActionDeliveredLater(params object?[] args) =>
            new(80315, ExceptionMessages.ReviewActionDeliveredLater, args) { HttpStatus = 422 };

        public static BusinessException FlightParameterNotFound(params object?[] args) =>
            new(80316, ExceptionMessages.FlightParameterNotFound, args) { HttpStatus = 422 };

        public static BusinessException SeriesRangeTooLarge(params object?[] args) =>
            new(80317, ExceptionMessages.SeriesRangeTooLarge, args) { HttpStatus = 422 };

        public static BusinessException FlightQualityNotEvaluated(params object?[] args) =>
            new(80318, ExceptionMessages.FlightQualityNotEvaluated, args) { HttpStatus = 404 };

        public static BusinessException FlightPhasesNotEvaluated(params object?[] args) =>
            new(80319, ExceptionMessages.FlightPhasesNotEvaluated, args) { HttpStatus = 404 };

        public static BusinessException SampleIndexOutOfRange(params object?[] args) =>
            new(80320, ExceptionMessages.SampleIndexOutOfRange, args) { HttpStatus = 422 };

        public static BusinessException RecomputeChangesNothing(params object?[] args) =>
            new(80321, ExceptionMessages.RecomputeChangesNothing, args) { HttpStatus = 422 };

        public static BusinessException RecomputeAlreadyPending(params object?[] args) =>
            new(80322, ExceptionMessages.RecomputeAlreadyPending, args) { HttpStatus = 409 };

        public static BusinessException DocumentNotPublishedOfKind(params object?[] args) =>
            new(80323, ExceptionMessages.DocumentNotPublishedOfKind, args) { HttpStatus = 422 };
    }
}
