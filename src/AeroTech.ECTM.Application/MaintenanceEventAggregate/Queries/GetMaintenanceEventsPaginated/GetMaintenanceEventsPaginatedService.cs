using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Services;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Queries.GetMaintenanceEventsPaginated
{
    public sealed class GetMaintenanceEventsPaginatedService : IGetMaintenanceEventsPaginatedService
    {
        private readonly IMaintenanceEventRepository _maintenanceEvents;
        private readonly MaintenanceEventDtoLoader _maintenanceEventDtos;

        public GetMaintenanceEventsPaginatedService(IMaintenanceEventRepository maintenanceEvents, MaintenanceEventDtoLoader maintenanceEventDtos)
        {
            _maintenanceEvents = maintenanceEvents;
            _maintenanceEventDtos = maintenanceEventDtos;
        }

        public async Task<PaginatedList<BackofficeMaintenanceEventDto>> ExecuteAsync(IMaintenanceEventsPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var page = await _maintenanceEvents.ListAsync(query.EngineId, query.AircraftId, query.Position, query.PageNumber, query.PageSize, cancellationToken);

            var maintenanceEventDtos = await _maintenanceEventDtos.LoadAsync(page.Results.ToList(), cancellationToken);
            var dtoById = maintenanceEventDtos.ToDictionary(maintenanceEvent => maintenanceEvent.Id);

            return page.Map(maintenanceEvent => dtoById[maintenanceEvent.Id]);
        }
    }
}
