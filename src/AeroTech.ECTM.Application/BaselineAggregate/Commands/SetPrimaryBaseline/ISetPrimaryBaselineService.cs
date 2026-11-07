using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.SetPrimaryBaseline
{
    public interface ISetPrimaryBaselineService
    {
        Task<BackofficeEngineBaselinesDto> SetAsync(ISetPrimaryBaselineCommand command, CancellationToken cancellationToken = default);
    }
}
