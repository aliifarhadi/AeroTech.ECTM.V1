namespace AeroTech.ECTM.RestApi.V1.BaselineAggregate.Requests
{
    public sealed record PublishBaselineRequest(int ExpectedDraftRevision, bool ConfirmedRawReference);
}
