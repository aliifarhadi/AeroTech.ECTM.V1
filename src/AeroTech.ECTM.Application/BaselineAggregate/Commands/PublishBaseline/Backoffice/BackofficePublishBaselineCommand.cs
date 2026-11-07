using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PublishBaseline.Backoffice
{
    public sealed record BackofficePublishBaselineCommand(
        long BaselineId,
        int ExpectedDraftRevision,
        bool ConfirmedRawReference) : IRequest<BackofficeBaselinePublicationDto>, IPublishBaselineCommand;
}
