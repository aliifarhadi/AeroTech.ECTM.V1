using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CreateBaseline
{
    public interface ICreateBaselineService
    {
        Task<BackofficeBaselineDto> CreateAsync(ICreateBaselineCommand command, CancellationToken cancellationToken = default);
    }
}
