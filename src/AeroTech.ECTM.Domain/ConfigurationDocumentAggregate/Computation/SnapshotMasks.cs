namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record SnapshotMasks(
        IReadOnlyList<string>? Untrusted,
        IReadOnlyList<string>? Coherence,
        bool? GapNeighboursUntrusted);
}
