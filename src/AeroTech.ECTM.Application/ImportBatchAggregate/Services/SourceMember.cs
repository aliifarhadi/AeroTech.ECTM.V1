namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed record SourceMember(string Name, string? ContainerName, byte[] Content);
}
