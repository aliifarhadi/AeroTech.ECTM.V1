namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.RetireBaseline
{
    public interface IRetireBaselineCommand
    {
        long BaselineId { get; }

        string Reason { get; }
    }
}
