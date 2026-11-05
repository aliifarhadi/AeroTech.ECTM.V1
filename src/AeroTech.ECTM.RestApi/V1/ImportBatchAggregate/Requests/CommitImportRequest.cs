namespace AeroTech.ECTM.RestApi.V1.ImportBatchAggregate.Requests
{
    public sealed record CommitImportRequest(long BatchId, string IdempotencyKey);
}
