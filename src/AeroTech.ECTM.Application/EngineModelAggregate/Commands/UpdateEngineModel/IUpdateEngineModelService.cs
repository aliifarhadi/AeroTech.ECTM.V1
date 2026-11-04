using AeroTech.ECTM.Application.EngineModelAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel
{
    public interface IUpdateEngineModelService
    {
        Task<BackofficeEngineModelDto> UpdateAsync(IUpdateEngineModelCommand command, CancellationToken cancellationToken = default);
    }
}
