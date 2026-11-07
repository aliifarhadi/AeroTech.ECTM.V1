using AeroTech.ECTM.Application.BaselineAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PublishBaseline
{
    public interface IPublishBaselineService
    {
        Task<BackofficeBaselinePublicationDto> PublishAsync(IPublishBaselineCommand command, CancellationToken cancellationToken = default);
    }
}
