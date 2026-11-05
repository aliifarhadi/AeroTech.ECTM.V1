using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Dto
{
    public sealed record BackofficeImportItemDto(
        long Id,
        int Ordinal,
        string FileName,
        string? ContainerName,
        string? Sha256,
        long SizeBytes,
        EnumValueDto Outcome,
        string? RejectionCode,
        string? RejectionMessage,
        long? ExistingFlightId,
        EnumValueDto State,
        string? ErrorCode,
        long? FlightId,
        EnumValueDto? FlightState,
        EnumValueDto? Grade,
        BackofficeFlightIdentityDto? Identity,
        int SampleCount,
        double? FirstElapsed,
        double? LastElapsed,
        int BlockingCount,
        int WarningCount,
        int InfoCount,
        IReadOnlyList<BackofficeValidationFindingDto> Findings,
        IReadOnlyList<HeaderMismatch> HeaderMismatches,
        IReadOnlyList<RecordingRow> FirstRows,
        IReadOnlyList<RecordingRow> LastRows);
}
