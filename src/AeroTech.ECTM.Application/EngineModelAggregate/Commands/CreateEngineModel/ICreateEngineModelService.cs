using AeroTech.ECTM.Application.EngineModelAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel
{
    public interface ICreateEngineModelService
    {
        Task<BackofficeEngineModelDto> CreateAsync(ICreateEngineModelCommand command, CancellationToken cancellationToken = default);
    }
}
