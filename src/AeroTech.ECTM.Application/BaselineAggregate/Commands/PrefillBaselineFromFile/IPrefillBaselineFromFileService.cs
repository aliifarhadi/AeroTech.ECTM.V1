using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PrefillBaselineFromFile
{
    public interface IPrefillBaselineFromFileService
    {
        Task<BackofficeBaselinePrefillDto> PrefillAsync(IPrefillBaselineFromFileCommand command, CancellationToken cancellationToken = default);
    }
}
