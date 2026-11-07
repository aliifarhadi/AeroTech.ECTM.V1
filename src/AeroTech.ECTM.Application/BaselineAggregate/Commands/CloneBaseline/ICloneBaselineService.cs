using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CloneBaseline
{
    public interface ICloneBaselineService
    {
        Task<BackofficeBaselineDto> CloneAsync(ICloneBaselineCommand command, CancellationToken cancellationToken = default);
    }
}
