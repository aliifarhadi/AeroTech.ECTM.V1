using AeroTech.ECTM.Domain._Shared;

namespace AeroTech.ECTM.Domain.FlightAggregate.Arguments
{
    public sealed record FlightRecordingArgs(
        long SourceFileId,
        long ImportBatchId,
        long ImportItemId,
        ConfigurationVersions Versions,
        string? TimeParameterKey,
        int SampleCount,
        double? FirstElapsed,
        double? LastElapsed);
}
