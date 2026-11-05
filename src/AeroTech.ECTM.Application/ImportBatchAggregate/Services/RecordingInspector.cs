using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Computation;
using AeroTech.Framework.Core.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed class RecordingInspector
    {
        private readonly ImportOptions _options;

        public RecordingInspector(IOptions<ImportOptions> options) => _options = options.Value;

        public RecordingInspection? RejectionOf(string fileName, long sizeBytes)
        {
            if (!SourceFileKinds.IsRecording(fileName))
                return RecordingInspection.Rejected(RejectionCodes.UnsupportedFileType, $"{fileName} is not a CSV or XLSX file.");

            if (sizeBytes > _options.MaximumFileBytes)
                return RecordingInspection.Rejected(RejectionCodes.FileTooLarge, $"{fileName} has {sizeBytes} bytes; the limit is {_options.MaximumFileBytes}.");

            if (SourceFileKinds.IsSynthetic(fileName) && !_options.SyntheticSourcesAllowed)
                return RecordingInspection.Rejected(RejectionCodes.SyntheticSourceNotAllowed, $"{fileName} is marked {SourceFileKinds.SyntheticMarker}; synthetic sources are accepted only in a test environment.");

            return null;
        }

        public RecordingInspection Inspect(string fileName, byte[] content, RecordingSet recordingSet)
        {
            if (RejectionOf(fileName, content.LongLength) is { } rejection)
                return rejection;

            ParsedRecording recording;

            try
            {
                var rows = SourceRows.Read(fileName, content, recordingSet.Delimiter, _options.MaximumFileBytes);
                recording = RecordingReader.Read(recordingSet.Profile, recordingSet.Catalog, rows);
            }
            catch (BusinessException exception) when (exception.Code is ExceptionFactory.SourceFileIsUnreadableCode or ExceptionFactory.SourceFileIsTooLargeCode)
            {
                return RecordingInspection.Rejected(RejectionCodes.SourceFileUnreadable, exception.Message);
            }

            if (!recording.SignatureMatches)
            {
                var message = $"The header does not match the active data profile: {recording.HeaderMismatches.Count} column(s) differ, {recording.HeaderColumnCount} of {recording.ExpectedColumnCount} columns present.";

                return RecordingInspection.Rejected(RejectionCodes.ProfileSignatureMismatch, message, recording.HeaderMismatches);
            }

            if (recording.SampleCount == 0)
                return RecordingInspection.Rejected(RejectionCodes.NoDataRows, $"{fileName} has a header but no data rows.");

            return RecordingInspection.Parsed(recording);
        }
    }
}
