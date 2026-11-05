using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed record RecordingInspection(
        ParsedRecording? Recording,
        string? RejectionCode,
        string? RejectionMessage,
        IReadOnlyList<HeaderMismatch> HeaderMismatches)
    {
        public bool IsRejected => RejectionCode is not null;

        public static RecordingInspection Parsed(ParsedRecording recording) => new(recording, null, null, []);

        public static RecordingInspection Rejected(string code, string message) => new(null, code, message, []);

        public static RecordingInspection Rejected(string code, string message, IReadOnlyList<HeaderMismatch> headerMismatches)
            => new(null, code, message, headerMismatches);
    }
}
