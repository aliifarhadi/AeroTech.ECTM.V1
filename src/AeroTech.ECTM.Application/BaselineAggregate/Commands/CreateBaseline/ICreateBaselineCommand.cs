namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CreateBaseline
{
    public interface ICreateBaselineCommand : IBaselineDraftInput
    {
        long EngineId { get; }
    }
}
