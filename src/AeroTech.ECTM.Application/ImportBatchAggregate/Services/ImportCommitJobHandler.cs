using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.ECTM.Domain.ImportBatchAggregate;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed class ImportCommitJobHandler : IProcessingJobHandler
    {
        public const string UnitFailed = "UNIT_FAILED";

        private readonly IImportBatchRepository _batches;
        private readonly ISourceFileRepository _sourceFiles;
        private readonly IFlightRepository _flights;
        private readonly RecordingSetLoader _recordingSets;
        private readonly RecordingInspector _inspector;
        private readonly FlightIdentityService _identities;
        private readonly AnalysisRulesLoader _analysisRules;
        private readonly AnalysisRecorder _analysisRecorder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public ImportCommitJobHandler(
            IImportBatchRepository batches,
            ISourceFileRepository sourceFiles,
            IFlightRepository flights,
            RecordingSetLoader recordingSets,
            RecordingInspector inspector,
            FlightIdentityService identities,
            AnalysisRulesLoader analysisRules,
            AnalysisRecorder analysisRecorder,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _batches = batches;
            _sourceFiles = sourceFiles;
            _flights = flights;
            _recordingSets = recordingSets;
            _inspector = inspector;
            _identities = identities;
            _analysisRules = analysisRules;
            _analysisRecorder = analysisRecorder;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public ProcessingJobKind Kind => ProcessingJobKind.IMPORT_COMMIT;

        public async Task<IReadOnlyList<long>> PrepareAsync(long subjectId, CancellationToken cancellationToken = default)
        {
            var batch = await BatchOfAsync(subjectId, cancellationToken);

            return batch.PendingItemIds();
        }

        public async Task ProcessAsync(long subjectId, long unitId, CancellationToken cancellationToken = default)
        {
            var batch = await BatchOfAsync(subjectId, cancellationToken);
            var item = batch.ItemOf(unitId);

            if (!item.IsPending)
                return;

            if (item.Outcome == ImportItemOutcome.REJECTED || item.SourceFileId is not { } sourceFileId)
            {
                batch.SkipItem(item.Id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            var existingFlight = await _flights.FindBySourceFileIdAsync(sourceFileId, cancellationToken);

            if (existingFlight is not null)
            {
                batch.LinkDuplicateItem(item.Id, existingFlight.Id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            await ImportFlightAsync(batch, item.Id, sourceFileId, cancellationToken);
        }

        public async Task AbandonAsync(long subjectId, long unitId, CancellationToken cancellationToken = default)
        {
            var batch = await BatchOfAsync(subjectId, cancellationToken);

            batch.FailItem(unitId, UnitFailed);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task FinishAsync(long subjectId, CancellationToken cancellationToken = default)
        {
            var batch = await BatchOfAsync(subjectId, cancellationToken);

            batch.FinishCommit(_clock.GetDateTime());

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task FailAsync(long subjectId, CancellationToken cancellationToken = default)
        {
            var batch = await BatchOfAsync(subjectId, cancellationToken);

            batch.FailCommit();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task ImportFlightAsync(ImportBatch batch, long itemId, long sourceFileId, CancellationToken cancellationToken)
        {
            var item = batch.ItemOf(itemId);
            var bytes = await _sourceFiles.GetBytesAsync(sourceFileId, cancellationToken)
                        ?? throw ExceptionFactory.SourceFileIsUnreadable($"source file {sourceFileId} has no content");
            var recordingSet = await _recordingSets.LoadPinnedAsync(batch.Versions, cancellationToken);
            var inspection = _inspector.Inspect(item.FileName, bytes, recordingSet);

            if (inspection.IsRejected)
            {
                batch.FailItem(item.Id, inspection.RejectionCode!);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            var recording = inspection.Recording!;
            var claim = IdentityClaims.From(item.FileName, batch.DefaultRegistration, batch.DefaultYear, recording);
            var identity = await _identities.ResolveAsync(claim, recording.EnginePositions, 0, false, cancellationToken);
            var recordingArgs = new FlightRecordingArgs(sourceFileId, batch.Id, item.Id, batch.Versions, recording.TimeChannel?.ParameterKey, recording.SampleCount, recording.FirstElapsed, recording.LastElapsed);
            var flight = Flight.Import(_idGenerator.NewId(), recordingArgs, identity, recording.Findings, recording.Channels, _idGenerator, _clock.GetDateTime());

            await _flights.AddAsync(flight, cancellationToken);
            await RecordAnalysisAsync(flight, recording, cancellationToken);
            batch.CommitItem(item.Id, flight.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task RecordAnalysisAsync(Flight flight, ParsedRecording recording, CancellationToken cancellationToken)
        {
            var rules = await _analysisRules.FindActiveAsync(cancellationToken);

            if (rules is null)
                return;

            var revision = await _analysisRecorder.OpenAsync(flight, rules, RevisionTrigger.IMPORT, cancellationToken);

            await _analysisRecorder.CompleteAsync(revision, rules, QualityInput.From(recording), cancellationToken);
        }

        private async Task<ImportBatch> BatchOfAsync(long batchId, CancellationToken cancellationToken)
            => await _batches.GetAsync(batchId, cancellationToken)
               ?? throw ExceptionFactory.ImportBatchNotFound(batchId);
    }
}
