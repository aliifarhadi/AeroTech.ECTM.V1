using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.ImportBatchAggregate;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Arguments;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Computation;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.PreviewImport
{
    public sealed class PreviewImportService : IPreviewImportService
    {
        private readonly UploadExpander _expander;
        private readonly RecordingSetLoader _recordingSets;
        private readonly RecordingInspector _inspector;
        private readonly FlightIdentityService _identities;
        private readonly ISourceFileRepository _sourceFiles;
        private readonly IFlightRepository _flights;
        private readonly IImportBatchRepository _batches;
        private readonly ImportBatchDtoLoader _batchDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public PreviewImportService(
            UploadExpander expander,
            RecordingSetLoader recordingSets,
            RecordingInspector inspector,
            FlightIdentityService identities,
            ISourceFileRepository sourceFiles,
            IFlightRepository flights,
            IImportBatchRepository batches,
            ImportBatchDtoLoader batchDtos,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _expander = expander;
            _recordingSets = recordingSets;
            _inspector = inspector;
            _identities = identities;
            _sourceFiles = sourceFiles;
            _flights = flights;
            _batches = batches;
            _batchDtos = batchDtos;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<BackofficeImportBatchDto> PreviewAsync(IPreviewImportCommand command, CancellationToken cancellationToken = default)
        {
            var recordingSet = await _recordingSets.LoadActiveAsync(cancellationToken);
            var members = _expander.Expand(command.Files);
            var sha256s = members.Select(member => SourceFile.Sha256Of(member.Content)).ToList();
            var now = _clock.GetDateTime();
            var batch = ImportBatch.Open(_idGenerator.NewId(), recordingSet.Versions, command.DefaultRegistration, command.DefaultYear, now);
            var context = await ContextOfAsync(sha256s, cancellationToken);
            var recordings = ReadInParallel(batch, members, sha256s, recordingSet, context);
            var rowsByItem = new Dictionary<long, ShownRows>();

            for (var index = 0; index < members.Count; index++)
            {
                var recording = recordings.GetValueOrDefault(index);
                var itemArgs = await PreviewMemberAsync(members[index], sha256s[index], recording, context, now, cancellationToken);
                var item = batch.AddItem(_idGenerator.NewId(), itemArgs);

                if (recording?.Rows is { } rows && itemArgs.Outcome == ImportItemOutcome.NEW)
                    rowsByItem[item.Id] = rows;
            }

            await _batches.AddAsync(batch, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _batchDtos.LoadAsync(batch, rowsByItem, cancellationToken);
        }

        private async Task<PreviewContext> ContextOfAsync(IReadOnlyList<string> sha256s, CancellationToken cancellationToken)
        {
            var storedFiles = await _sourceFiles.ListBySha256Async(sha256s.Distinct().ToList(), cancellationToken);
            var flights = await _flights.ListBySourceFileIdsAsync(storedFiles.Select(sourceFile => sourceFile.Id).ToList(), cancellationToken);

            return new PreviewContext(
                storedFiles.ToDictionary(sourceFile => sourceFile.Sha256, sourceFile => sourceFile.Id),
                flights.ToDictionary(flight => flight.SourceFileId, flight => flight.Id));
        }

        private Dictionary<int, MemberRecording> ReadInParallel(ImportBatch batch, IReadOnlyList<SourceMember> members, IReadOnlyList<string> sha256s, RecordingSet recordingSet, PreviewContext context)
        {
            var firstIndexBySha256 = new Dictionary<string, int>(StringComparer.Ordinal);

            for (var index = 0; index < members.Count; index++)
                firstIndexBySha256.TryAdd(sha256s[index], index);

            var indexes = Enumerable.Range(0, members.Count)
                .Where(index => firstIndexBySha256[sha256s[index]] == index)
                .Where(index => !context.HasFlight(sha256s[index]))
                .ToList();
            var recordings = new MemberRecording[members.Count];

            Parallel.ForEach(indexes, index => recordings[index] = Read(batch, members[index], recordingSet));

            return indexes.ToDictionary(index => index, index => recordings[index]);
        }

        private MemberRecording Read(ImportBatch batch, SourceMember member, RecordingSet recordingSet)
        {
            var inspection = _inspector.Inspect(member.Name, member.Content, recordingSet);

            if (inspection.IsRejected)
                return new MemberRecording(inspection, null, [], null, null);

            var recording = inspection.Recording!;
            var claim = IdentityClaims.From(member.Name, batch.DefaultRegistration, batch.DefaultYear, recording);
            var summary = new ImportItemPreview(null, recording.SampleCount, recording.FirstElapsed, recording.LastElapsed, recording.Findings, [], null);
            var rows = new ShownRows(recording.FirstRows, recording.LastRows);

            return new MemberRecording(null, claim, recording.EnginePositions, summary, rows);
        }

        private async Task<ImportItemArgs> PreviewMemberAsync(
            SourceMember member,
            string sha256,
            MemberRecording? recording,
            PreviewContext context,
            DateTimeOffset now,
            CancellationToken cancellationToken)
        {
            if (_inspector.RejectionOf(member.Name, member.Content.LongLength) is { } rejection)
                return Rejected(member, sha256, rejection);

            var knownSourceFileId = context.SourceFileIdBySha256.GetValueOrDefault(sha256);

            if (knownSourceFileId != 0 && context.FlightIdBySourceFile.TryGetValue(knownSourceFileId, out var existingFlightId))
                return Duplicate(member, sha256, knownSourceFileId, existingFlightId);

            if (knownSourceFileId != 0 && context.Sha256InBatch.Contains(sha256))
                return Duplicate(member, sha256, knownSourceFileId, null);

            if (recording!.Rejection is { } parseRejection)
                return Rejected(member, sha256, parseRejection);

            var claim = recording.Claim!;
            var duplicateInBatch = IdentityKeyOf(claim) is { } identityKey && !context.IdentitiesInBatch.Add(identityKey);
            var identity = await _identities.ResolveAsync(claim, recording.EnginePositions, 0, duplicateInBatch, cancellationToken);
            var sourceFileId = await StoredSourceFileIdAsync(member, sha256, knownSourceFileId, context, now, cancellationToken);
            var preview = recording.Summary! with { Identity = identity };

            context.Sha256InBatch.Add(sha256);

            return new ImportItemArgs(member.Name, member.ContainerName, member.Content.LongLength, sha256, sourceFileId, ImportItemOutcome.NEW, null, null, preview);
        }

        private async Task<long> StoredSourceFileIdAsync(SourceMember member, string sha256, long knownSourceFileId, PreviewContext context, DateTimeOffset now, CancellationToken cancellationToken)
        {
            if (knownSourceFileId != 0)
                return knownSourceFileId;

            var sourceFile = SourceFile.Store(_idGenerator.NewId(), member.Name, member.Content, now);

            await _sourceFiles.AddAsync(sourceFile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            context.SourceFileIdBySha256[sha256] = sourceFile.Id;

            return sourceFile.Id;
        }

        private static ImportItemArgs Duplicate(SourceMember member, string sha256, long sourceFileId, long? existingFlightId)
        {
            var preview = new ImportItemPreview(null, 0, null, null, [], [], null);

            return new ImportItemArgs(member.Name, member.ContainerName, member.Content.LongLength, sha256, sourceFileId, ImportItemOutcome.DUPLICATE, null, existingFlightId, preview);
        }

        private static ImportItemArgs Rejected(SourceMember member, string sha256, RecordingInspection inspection)
        {
            var preview = ImportItemPreview.Rejected(inspection.RejectionMessage!, inspection.HeaderMismatches);

            return new ImportItemArgs(member.Name, member.ContainerName, member.Content.LongLength, sha256, null, ImportItemOutcome.REJECTED, inspection.RejectionCode, null, preview);
        }

        private static string? IdentityKeyOf(IdentityClaim claim)
        {
            if (claim.Registration is null || claim.FlightDate is null || claim.FlightNumber is null)
                return null;

            return $"{claim.Registration}|{claim.FlightDate:yyyy-MM-dd}|{claim.FlightNumber}";
        }

        private sealed record MemberRecording(
            RecordingInspection? Rejection,
            IdentityClaim? Claim,
            IReadOnlyList<int> EnginePositions,
            ImportItemPreview? Summary,
            ShownRows? Rows);

        private sealed record PreviewContext(Dictionary<string, long> SourceFileIdBySha256, Dictionary<long, long> FlightIdBySourceFile)
        {
            public HashSet<string> Sha256InBatch { get; } = new(StringComparer.Ordinal);

            public HashSet<string> IdentitiesInBatch { get; } = new(StringComparer.OrdinalIgnoreCase);

            public bool HasFlight(string sha256)
                => SourceFileIdBySha256.TryGetValue(sha256, out var sourceFileId) && FlightIdBySourceFile.ContainsKey(sourceFileId);
        }
    }
}
