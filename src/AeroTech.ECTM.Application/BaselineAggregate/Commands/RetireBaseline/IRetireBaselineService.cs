using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.RetireBaseline
{
    public interface IRetireBaselineService
    {
        Task<BackofficeBaselineDto> RetireAsync(IRetireBaselineCommand command, CancellationToken cancellationToken = default);
    }
}
