namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.UpdateBaseline
{
    public interface IUpdateBaselineCommand : IBaselineDraftInput
    {
        long BaselineId { get; }

        int ExpectedDraftRevision { get; }
    }
}
