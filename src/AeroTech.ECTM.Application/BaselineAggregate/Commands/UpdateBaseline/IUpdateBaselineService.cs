using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.UpdateBaseline
{
    public interface IUpdateBaselineService
    {
        Task<BackofficeBaselineDto> UpdateAsync(IUpdateBaselineCommand command, CancellationToken cancellationToken = default);
    }
}
