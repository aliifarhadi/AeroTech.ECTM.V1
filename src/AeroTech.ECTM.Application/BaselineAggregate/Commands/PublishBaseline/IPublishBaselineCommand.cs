namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PublishBaseline
{
    public interface IPublishBaselineCommand
    {
        long BaselineId { get; }

        int ExpectedDraftRevision { get; }

        bool ConfirmedRawReference { get; }
    }
}
